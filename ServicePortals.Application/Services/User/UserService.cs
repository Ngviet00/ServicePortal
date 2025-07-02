using System.Data;
using System.Text;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Hubs;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly OrgChartService _orgChartBuilder;
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly ICacheService _cacheService;

        public UserService(
            ApplicationDbContext context,
            NotificationService notificationService,
            OrgChartService orgChartBuilder,
            IViclockDapperContext viclockDapperContext,
            ICacheService cacheService
        )
        {
            _context = context;
            _notificationService = notificationService;
            _orgChartBuilder = orgChartBuilder;
            _viclockDapperContext = viclockDapperContext;
            _cacheService = cacheService;
        }

        public async Task<PagedResults<GetAllUserResponse>> GetAll(GetAllUserRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;
            int? DepartmentId = request.DepartmentId;
            int? Sex = request.Sex;
            int? PositionId = request.PositionId;

            var param = new
            {
                SearchName = name,
                SearchDept = DepartmentId,
                SearchSex = Sex,
                SearchPosition = PositionId,
                PageNumber = (int)page,
                PageSize = (int)pageSize
            };

            var whereSql = new StringBuilder();

            whereSql.AppendLine(" WHERE 1=1 AND nv.NVNgayRa > GETDATE()");

            if (!string.IsNullOrWhiteSpace(name))
            {
                whereSql.AppendLine($" AND nv.NVHoTen LIKE '%' + @SearchName + '%' ");
                whereSql.AppendLine($" OR u.UserCode LIKE '%' + @SearchName + '%' ");
                whereSql.AppendLine($" OR nv.NVDienThoai LIKE '%' + @SearchName + '%' ");
                whereSql.AppendLine($" OR nv.NVEmail LIKE '%' + @SearchName + '%' ");
            }

            if (DepartmentId != null)
            {
                whereSql.AppendLine($" AND nv.NVMaBP = @SearchDept ");
            }

            if (Sex != null)
            {
                whereSql.AppendLine($" AND nv.NVGioiTinh = @SearchSex ");
            }

            if (PositionId != null)
            {
                whereSql.AppendLine($" AND nv.NVMaCV = @SearchPosition ");
            }

            var countSql = new StringBuilder();
            countSql.AppendLine($@"
                SELECT COUNT(*) 
                FROM [{Global.DbWeb}].dbo.users u 
                INNER JOIN[{Global.DbViClock}].dbo.tblNhanVien nv ON u.UserCode = nv.NVMaNV 
                {whereSql}
            ");

            int totalItems = await _viclockDapperContext.QueryFirstOrDefaultAsync<int>(countSql.ToString(), param);
            int totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var dataSql = new StringBuilder();
            dataSql.AppendLine($@"
                WITH PagedUsers AS (
                    SELECT
                        u.Id,
                        u.UserCode,
                        [{Global.DbViClock}].dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
                        nv.NVMaBP,
                        bp.BPTen,
                        cv.CVTen,
                        nv.NVMaCV,
                        nv.NVGioiTinh,
                        nv.NVDienThoai,
                        nv.NVEmail,
                        nv.NVNgayVao
                    FROM [{Global.DbWeb}].dbo.users u
                    INNER JOIN [{Global.DbViClock}].dbo.tblNhanVien nv ON u.UserCode = nv.NVMaNV
                    LEFT JOIN [{Global.DbViClock}].dbo.tblBoPhan bp ON nv.NVMaBP = bp.BPMa
                    LEFT JOIN [{Global.DbViClock}].dbo.tblChucVu cv ON nv.NVMaCV = cv.CVMa
                    {whereSql}
                    ORDER BY u.Id
                    OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
                )
                SELECT 
                    pu.Id,
                    pu.UserCode,
                    pu.NVHoTen,
                    pu.NVMaBP,
                    pu.BPTen,
                    pu.CVTen,
                    pu.NVMaCV,
                    pu.NVGioiTinh,
                    pu.NVDienThoai,
                    pu.NVEmail,
                    pu.NVNgayVao,
                    r.Id AS RoleId,
                    r.Name AS RoleName
                FROM PagedUsers pu
                LEFT JOIN [{Global.DbWeb}].dbo.user_roles ur ON pu.UserCode = ur.UserCode
                LEFT JOIN [{Global.DbWeb}].dbo.roles r ON ur.RoleId = r.Id
            ");

            var users = await _viclockDapperContext.QueryAsync<GetAllUserResponse>(dataSql.ToString(), param);

            var results = users
                .GroupBy(x => x.Id)
                .Select(g => new GetAllUserResponse
                {
                    Id = g.First().Id,
                    UserCode = g.First().UserCode,
                    NVHoTen = g.First().NVHoTen,
                    NVMaBP = g.First().NVMaBP,
                    BPTen = g.First().BPTen,
                    CVTen = g.First().CVTen,
                    NVMaCV = g.First().NVMaCV,
                    NVGioiTinh = g.First().NVGioiTinh,
                    NVDienThoai = g.First().NVDienThoai,
                    NVEmail = g.First().NVEmail,
                    NVNgayVao = g.First().NVNgayVao,
                    Roles = g
                        .Where(r => r.RoleId.HasValue)
                        .Select(r => new Domain.Entities.Role
                        {
                            Id = r.RoleId.GetValueOrDefault(),
                            Name = r.RoleName
                        }).ToList()
                })
                .ToList();

            return new PagedResults<GetAllUserResponse>
            {
                Data = results,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<UserResponse> GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException("Code can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == code) ?? throw new NotFoundException("User not found!");

            return UserMapper.ToDto(user);
        }

        public async Task<UserResponse> GetById(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            return UserMapper.ToDto(user);
        }

        public async Task<UserResponse> Delete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<UserResponse> ForceDelete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<bool> UpdateUserRole(UpdateUserRoleRequest dto)
        {
            try
            {
                List<UserRole> ur = new List<UserRole>();

                var oldUserRoles = await _context.UserRoles.Where(e => e.UserCode == dto.UserCode).ToListAsync();
                _context.UserRoles.RemoveRange(oldUserRoles);

                if (dto?.RoleIds != null)
                {
                    foreach (var item in dto.RoleIds)
                    {
                        ur.Add(new UserRole
                        {
                            UserCode = dto.UserCode,
                            RoleId = item
                        });
                    }
                }

                _context.UserRoles.AddRange(ur);

                await _context.SaveChangesAsync();

                await _notificationService.SendMessageToUser(dto?.UserCode ?? "", "login_again");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error can not update user role, error: {ex.Message}");
                return false;
            }
        }

        public async Task<UserResponse> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found!");

            var password = !string.IsNullOrWhiteSpace(request.Password) ? request.Password : request.UserCode ?? "123456";

            user.Password = Helper.HashString(password);

            user.IsChangePassword = 0;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            string bodyMail = $@"
                <h2>Your Password Has Been Reset</h2>
                <div style=""font-size: 18px;"">
                    An administrator has reset your password. You can now log in using the password below: <br/>
                </div>
                <div style=""font-size: 25px;margin-top: 10px; color: #e71a1a;font-family: monospace;letter-spacing: 1px"">
                    {password}
                </div>
                <div style=""font-size: 18px;margin-top: 10px;"">
                    For security reasons, please change your password after logging in. <br/> <br/>
                    Thanks, <br/><br/>
                    MIS/IT Team
                </div>";

            BackgroundJob.Enqueue<IEmailService>(job => 
                job.SendEmailAsync(
                    new List<string> { user.Email ?? "" },
                    null,
                    "Reset password",
                    bodyMail,
                    null,
                    true
                )
            );

            return UserMapper.ToDto(user);
        }

        public async Task<OrgChartRequest> BuildTree(int? departmentId)
        {

            return null;
            //return await _orgChartBuilder.BuildTree(departmentId);
        }

        public async Task<GetUserPersonalInfoResponse?> GetMe(string UserCode)
        {
            var result = await _cacheService.GetOrCreateAsync($"user_info_{UserCode}", async () =>
            {
                return await _viclockDapperContext.QueryFirstOrDefaultAsync<GetUserPersonalInfoResponse>(
                    "dbo.usp_GetNhanVienByUserCode",
                    new { UserCode },
                    CommandType.StoredProcedure
                );
            }, expireMinutes: 30);

            return result;
        }

        public async Task<bool> CheckUserIsExistsInViClock(string UserCode)
        {
            var result = await _viclockDapperContext.QueryFirstOrDefaultAsync<int>("SELECT 1 FROM tblNhanVien WHERE NVMaNV = @UserCode", new { UserCode });

            return result == 1;
        }

        public async Task<List<GetEmailByUserCodeAndUserConfigResponse>> GetEmailByUserCodeAndUserConfig(List<string> userCodes)
        {
            return null;
            //if (userCodes == null || userCodes.Count == 0)
            //{
            //    return new List<GetEmailByUserCodeAndUserConfigResponse>();
            //}

            //var result = await _context.Users
            //    .Where(u => userCodes.Contains(u.UserCode ?? ""))
            //    .GroupJoin(
            //        _context.UserConfigs.Where(c => c.ConfigKey == "RECEIVE_MAIL_LEAVE_REQUEST"),
            //        u => u.UserCode,
            //        uc => uc.UserCode,
            //        (u, ucs) => new { u, uc = ucs.FirstOrDefault() }
            //    )
            //    .Where(x => x.uc == null || x.uc.ConfigValue == "true")
            //    .Select(x => new GetEmailByUserCodeAndUserConfigResponse
            //    {
            //        UserCode = x.u.UserCode,
            //        Email = x.u.Email,
            //        ConfigKey = x.uc != null ? x.uc.ConfigKey : null,
            //        ConfigValue = x.uc != null ? x.uc.ConfigValue : null
            //    })
            //    .ToListAsync();

            //return result;
        }

        public async Task<object?> GetCustomColumnUserViclockByUserCode(string userCode, string columns)
        {
            var sql = $@"SELECT
                            [{Global.DbViClock}].dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen,
                            {columns}
                        FROM [{Global.DbViClock}].[dbo].[tblNhanVien]
                        WHERE
                            NVMaNV = @UserCode";

            return await _viclockDapperContext.QueryFirstOrDefaultAsync<object?>(sql, new { UserCode = userCode });
        }
        public async Task<GetDetailInfoUserResponse> GetDetailUserWithRoleAndPermission(string userCode)
        {
            GetDetailInfoUserResponse result = new();

            var infoFromViclock = await GetCustomColumnUserViclockByUserCode(userCode, "NVMaBP, OrgUnitID, NVNgayVao, NVGioiTinh, NVEmail, NVDienThoai, NVNgaySinh");

            var user = await GetRoleAndPermissionByUser(userCode);

            if (user == null)
            {
                return result;
            }

            var userRoleAndPermissions = FormatRoleAndPermissionByUser(user);

            result.InfoFromViclock = infoFromViclock;
            result.User = user != null ? UserMapper.ToDto(user) : null;
            result.Roles = userRoleAndPermissions.Roles;
            result.Permissions = userRoleAndPermissions.Permissions;

            return result;
        }
        public async Task<Domain.Entities.User?> GetRoleAndPermissionByUser(string userCode)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .Include(u => u.UserPermissions)
                    .ThenInclude(up => up.Permission)
                .Where(e => e.UserCode == userCode)
                .FirstOrDefaultAsync();

            return user;
        }
        public UserRolesAndPermissionsResponse FormatRoleAndPermissionByUser(Domain.Entities.User user)
        {
            if (user == null)
            {
                return new UserRolesAndPermissionsResponse();
            }

            var result = new UserRolesAndPermissionsResponse();

            if (user.UserRoles != null)
            {
                foreach (var userRole in user.UserRoles)
                {
                    var role = userRole.Role;

                    if (role != null)
                    {
                        if (!string.IsNullOrWhiteSpace(role.Name))
                        {
                            result.Roles.Add(role.Name);
                        }

                        if (role.RolePermissions != null)
                        {
                            foreach (var rolePermission in role.RolePermissions)
                            {
                                if (!string.IsNullOrWhiteSpace(rolePermission.Permission?.Name))
                                {
                                    result.Permissions.Add(rolePermission.Permission.Name);
                                }
                            }
                        }
                    }
                }
            }

            if (user.UserPermissions != null)
            {
                foreach (var userPermission in user.UserPermissions)
                {
                    if (!string.IsNullOrWhiteSpace(userPermission.Permission?.Name))
                    {
                        result.Permissions.Add(userPermission.Permission.Name);
                    }
                }
            }

            return result;
        }
    }
}
