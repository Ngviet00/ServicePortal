using System.Data;
using System.Text;
using Dapper;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;

        public UserService(
            ApplicationDbContext context,
            ICacheService cacheService
        )
        {
            _context = context;
            _cacheService = cacheService;
        }

        //lấy danh sách user, kết hợp vs tblNhanVien bên db viclock
        public async Task<PagedResults<GetAllUserResponse>> GetAll(GetAllUserRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SearchDepartmentId", request.DepartmentId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SearchGender", request.Sex, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SearchName", Helper.RemoveDiacritics(request.Name ?? ""), DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchStatus", request?.Status ?? null, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                    .QueryAsync<GetAllUserResponse>(
                        "dbo.sp_GetListUser",
                        parameters,
                        commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<GetAllUserResponse>
            {
                Data = (List<GetAllUserResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Lấy chi tiết thông tin user bao gồm role và permission
        /// </summary>
        public async Task<DetailUserWithRoleAndPermissionResponse> GetDetailUserWithRoleAndPermission(string userCode)
        {
            DetailUserWithRoleAndPermissionResponse result = new();

            var rolesOfUser = await _context.UserRoles
                .Where(ur => ur.UserCode == userCode)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .ToListAsync();

            var permissionOfUser = await _context.UserPermissions
                .Where(up => up.UserCode == userCode)
                .Include(up => up.Permission)
                .Select(up => up.Permission)
                .ToListAsync();

            result.UserCode = userCode;

            result.Roles = rolesOfUser;

            result.Permissions = permissionOfUser;

            return result;
        }

        //Cập nhật user role
        public async Task<bool> UpdateUserRole(UpdateUserRoleRequest dto)
        {
            try
            {
                List<UserRole> userRoles = [];

                var oldUserRoles = await _context.UserRoles.Where(e => e.UserCode == dto.UserCode).ToListAsync();
                _context.UserRoles.RemoveRange(oldUserRoles);

                foreach (var item in dto.RoleIds ?? [])
                {
                    userRoles.Add(new UserRole
                    {
                        UserCode = dto.UserCode,
                        RoleId = item
                    });
                }

                _context.UserRoles.AddRange(userRoles);

                await _context.SaveChangesAsync();

                _cacheService.Remove($"user_info_{dto.UserCode}");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error can not update user role, error: {ex.Message}");
                return false;
            }
        }

        //cập nhật user permission
        public async Task<bool> UpdateUserPermission(UpdateUserRoleRequest dto)
        {
            try
            {
                List<UserPermission> userPermissions = [];

                var oldUserPermissions = await _context.UserPermissions.Where(e => e.UserCode == dto.UserCode).ToListAsync();
                _context.UserPermissions.RemoveRange(oldUserPermissions);

                foreach (var item in dto.PermissionIds ?? [])
                {
                    userPermissions.Add(new UserPermission
                    {
                        UserCode = dto.UserCode,
                        PermissionId = item
                    });
                }

                _context.UserPermissions.AddRange(userPermissions);

                await _context.SaveChangesAsync();

                _cacheService.Remove($"user_info_{dto.UserCode}");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error can not update user permission, error: {ex.Message}");
                return false;
            }
        }

        //reset pw, khi reset xong, người dùng đăng nhập lại bắt buộc phải đổi mật khẩu -> khi đổi xong IsChangePassword = 1
        public async Task<UserResponse> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found!");

            var password = !string.IsNullOrWhiteSpace(request.Password) ? request.Password : request.UserCode ?? "123456";

            user.Password = Helper.HashString(password);

            user.IsChangePassword = 0;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            string bodyMail = TemplateEmail.EmailResetPassword(password);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailResetPassword(
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

        //tìm kiếm user theo usercode
        public async Task<UserResponse?> GetByUserCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException("UserCode can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == code);

            return user != null ? UserMapper.ToDto(user) : null;
        }

        //tìm kiếm user theo id
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

        //lấy thông tin cá nhân của user, kết hợp cả thông tin bên viclock, cache tầm 2 phút
        public async Task<PersonalInfoResponse?> GetMe(string userCode)
        {
            var result = await _cacheService.GetOrCreateAsync($"user_info_{userCode}", async () =>
            {
                var infoFromDb = await _context.Database.GetDbConnection()
                    .QueryFirstOrDefaultAsync<PersonalInfoResponse>(
                        "dbo.GetUserByUserCode",
                        new { userCode },
                        commandType: CommandType.StoredProcedure
                    );

                var info = infoFromDb ?? new PersonalInfoResponse();

                var roleAndPermissionUser = await GetRoleAndPermissionByUser(userCode);
                var formatRoleAndPermission = FormatRoleAndPermissionByUser(roleAndPermissionUser);

                info.Roles = formatRoleAndPermission.Roles;
                info.Permissions = formatRoleAndPermission.Permissions;

                return info;
            }, expireMinutes: 2);

            return result;
        }

        //lấy thông tin user theo mã nhân viên ở db viclock, có thể tùy chỉnh thêm cột nếu muốn
        public async Task<object?> GetCustomColumnUserViclockByUserCode(string userCode, string columns)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"SELECT
                            NVMa,
                            NVMaNV,
                            [{Global.DbViClock}].dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen,
                            {columns}
                        FROM [{Global.DbViClock}].[dbo].[tblNhanVien]
                        WHERE
                            NVMaNV = @UserCode";

            var result = await connection.QueryFirstOrDefaultAsync<object?>(sql, new { UserCode = userCode });

            return result;
        }

        //lấy role và permission theo usercode
        public async Task<Domain.Entities.User?> GetRoleAndPermissionByUser(string userCode)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(e => e.UserCode == userCode);

            var userPermissions = await _context.UserPermissions
                .Where(up => up.UserCode == userCode)
                .Include(up => up.Permission)
                .ToListAsync();

            if (user != null)
            {
                user.UserPermissions = userPermissions;
            }

            return user;
        }
        public UserRolesAndPermissionsResponse FormatRoleAndPermissionByUser(Domain.Entities.User? user)
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
        public async Task<UserResponse> Update(string userCode, UpdatePersonalInfoRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == userCode) ?? throw new NotFoundException("User not found!");

            user.Id = user.Id;
            user.Phone = request.Phone;
            user.Email = request.Email;
            user.DateOfBirth = request.DateOfBirth;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _cacheService.Remove($"user_info_{userCode}");

            return UserMapper.ToDto(user);
        }

        /// <summary>
        /// Lấy những người theo orgUnitId kết hợp bảng user vs tblNhanVien bên db viclock
        /// </summary>
        public async Task<List<GetMultiUserViClockByOrgPositionIdResponse>> GetMultipleUserViclockByOrgPositionId(int OrgPositionId, List<string>? UserCodes = null)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sql = $@"
                SELECT
                    NVMa,
                    NVMaNV,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen,
                    ViTriToChucId AS OrgPositionId,
	                COALESCE(NULLIF(Email, ''), NVEmail, '') AS Email
                FROM {Global.DbViClock}.[dbo].[tblNhanVien] AS NV
                LEFT JOIN {Global.DbWeb}.dbo.users as U
	                ON NV.NVMaNV = U.UserCode
                WHERE
                    NV.NVNgayRa > GETDATE()
            ";

            if (UserCodes != null && UserCodes.Any())
            {
                sql += " AND NV.NVMaNV IN @UserCodes";
            }
            else
            {
                sql += " AND NV.ViTriToChucId = @OrgPositionId";
            }

            var result = await connection.QueryAsync<GetMultiUserViClockByOrgPositionIdResponse>(sql, new { OrgPositionId, UserCodes });

            return (List<GetMultiUserViClockByOrgPositionIdResponse>)result;
        }

        //xây dựng hierarchy sơ đồ tổ chức
        public async Task<List<TreeNode>> BuildOrgTree(int departmentId)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var param = new
            {
                DepartmentId = departmentId
            };

            var sql = $@"
                SELECT
                    NV.NVMaNV,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
                    OP.Id AS OrgPositionId,
                    OP.Name AS PositionName,
                    OP.ParentOrgPositionId,
                    OU.Name AS TeamName
                FROM {Global.DbWeb}.dbo.org_units AS OU
                INNER JOIN {Global.DbWeb}.dbo.org_positions AS OP ON OU.Id = OP.OrgUnitId
                LEFT JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV ON OP.Id = NV.ViTriToChucId
                WHERE (OU.ParentOrgUnitId = @DepartmentId OR OP.OrgUnitId = @DepartmentId)
            ";

            var results = (await connection.QueryAsync<TreeNode>(sql, param)).ToList();

            var lookup = results.Where(x => x.OrgPositionId.HasValue).GroupBy(x => x.OrgPositionId.Value).ToDictionary(g => g.Key, g => g.First());

            var roots = results.Where(n => !n.ParentOrgPositionId.HasValue || !lookup.ContainsKey(n.ParentOrgPositionId.Value)).ToList();

            if (roots.Count == 0 && results.Count != 0)
            {
                roots.Add(results.First());
            }

            foreach (var n in results)
            {
                if (n.ParentOrgPositionId.HasValue && lookup.TryGetValue(n.ParentOrgPositionId.Value, out var parent) && parent != n)
                {
                    parent.Children.Add(n);
                }
            }

            return roots;
        }

        //tìm kiếm tất cả người dùng ở bên viclock
        public async Task<PagedResults<object>> SearchAllUserFromViClock(SearchAllUserFromViclockRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var param = new
            {
                Key = request.Keysearch,
                request.Page,
                request.PageSize,
            };

            StringBuilder sbWhere = new();

            sbWhere.AppendLine(" WHERE 1=1 AND NV.NVNgayRa > GETDATE()");

            if (!string.IsNullOrWhiteSpace(request.Keysearch))
            {
                sbWhere.AppendLine($" AND ({Global.DbViClock}.dbo.udf_RemoveDiacritics({Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen)) LIKE '%' + @Key + '%' ");
                sbWhere.AppendLine($" OR NV.NVMaNV LIKE '%' + @Key + '%' )");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT");
            sb.AppendLine("     NV.NVMaNV,");
            sb.AppendLine($"     {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen,");
            sb.AppendLine("     BP.BPMa,");
            sb.AppendLine($"     {Global.DbViClock}.dbo.funTCVN2Unicode(BP.BPTen) as BPTen,");
            sb.AppendLine("     NV.NVNgayVao");
            sb.AppendLine($" FROM {Global.DbViClock}.dbo.tblNhanVien as NV");

            sb.AppendLine($" LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan as BP");
            sb.AppendLine(" ON NV.NVMaBP = BP.BPMa");

            sb.AppendLine($@" {sbWhere}");

            sb.AppendLine($@" GROUP BY
                NV.NVMaNV,
                NV.NVHoTen,
                BP.BPMa,
                BP.BPTen,
                NV.NVNgayVao"
            );

            sb.AppendLine(" ORDER BY NV.NVHoTen ASC");
            sb.AppendLine(" OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY");

            var countSql = new StringBuilder();
            countSql.AppendLine(" SELECT COUNT(*)");
            countSql.AppendLine($" FROM {Global.DbViClock}.dbo.tblNhanVien AS NV");
            countSql.AppendLine($" LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan as BP ON NV.NVMaBP = BP.BPMa");
            countSql.AppendLine($"{sbWhere}");

            int totalItems = await connection.QueryFirstOrDefaultAsync<int>(countSql.ToString(), param);

            int totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var data = await connection.QueryAsync<object>(sb.ToString(), param);

            return new PagedResults<object>
            {
                Data = (List<object>)data,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<PersonalInfoResponse> SearchUserCombineViClockAndWebSystem(string userCode)
        {
            var infoFromDb = await _context.Database.GetDbConnection()
                    .QueryFirstOrDefaultAsync<PersonalInfoResponse>(
                        "dbo.GetUserByUserCode",
                        new { userCode },
                        commandType: CommandType.StoredProcedure
                    );

            var info = infoFromDb ?? new PersonalInfoResponse();

            return info;
        }

        public async Task<object> Test()
        {
            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.PositonContext == "MANAGER").ToListAsync();

            return approvalFlows;
        }
    }
}
