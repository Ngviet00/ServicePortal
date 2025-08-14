using System.Data;
using System.Text;
using Azure.Core;
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
            return new PagedResults<GetAllUserResponse>
            {
                Data = [],
                TotalItems = 0,
                TotalPages = 0
            };
            //string name = request.Name ?? "";
            //double pageSize = request.PageSize;
            //double page = request.Page;
            //string? DepartmentName = request.DepartmentName;
            //int? Sex = request.Sex;
            //int? PositionId = request.PositionId;

            //var param = new
            //{
            //    SearchName = Helper.RemoveDiacritics(name),
            //    SearchDept = DepartmentName,
            //    SearchSex = Sex,
            //    SearchPosition = PositionId,
            //    PageNumber = (int)page,
            //    PageSize = (int)pageSize
            //};

            //var whereSql = new StringBuilder();

            //whereSql.AppendLine(" WHERE 1=1 AND nv.NVNgayRa > GETDATE()");

            //if (!string.IsNullOrWhiteSpace(name))
            //{
            //    whereSql.AppendLine($" AND (dbo.udf_RemoveDiacritics(dbo.funTCVN2Unicode(nv.NVHoTen)) LIKE '%' + @SearchName + '%' ");
            //    whereSql.AppendLine($" OR u.UserCode LIKE '%' + @SearchName + '%' )");
            //}

            //if (DepartmentName != null)
            //{
            //    whereSql.AppendLine($" AND bp.BPTen = @SearchDept ");
            //}

            //if (Sex != null)
            //{
            //    whereSql.AppendLine($" AND nv.NVGioiTinh = @SearchSex ");
            //}

            //if (PositionId != null)
            //{
            //    whereSql.AppendLine($" AND nv.NVMaCV = @SearchPosition ");
            //}

            //var countSql = new StringBuilder();
            //countSql.AppendLine($@"
            //    SELECT COUNT(*) 
            //    FROM [{Global.DbWeb}].dbo.users u 
            //    INNER JOIN[{Global.DbViClock}].dbo.tblNhanVien nv ON u.UserCode = nv.NVMaNV 
            //    LEFT JOIN [{Global.DbViClock}].dbo.tblBoPhan bp ON nv.NVMaBP = bp.BPMa
            //    {whereSql}
            //");

            //int totalItems = await _viclockDapperContext.QueryFirstOrDefaultAsync<int>(countSql.ToString(), param);
            //int totalPages = (int)Math.Ceiling(totalItems / pageSize);

            //var dataSql = new StringBuilder();
            //dataSql.AppendLine($@"
            //    WITH PagedUsers AS (
            //        SELECT
            //            u.Id,
            //            u.UserCode,
            //            [{Global.DbViClock}].dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
            //            nv.NVMaBP,
            //            bp.BPTen,
            //            cv.CVTen,
            //            nv.NVMaCV,
            //            nv.NVGioiTinh,
            //            u.Phone,
            //            u.Email,
            //            u.DateOfBirth,
            //            nv.NVNgayVao
            //        FROM [{Global.DbWeb}].dbo.users u
            //        INNER JOIN [{Global.DbViClock}].dbo.tblNhanVien nv ON u.UserCode = nv.NVMaNV
            //        LEFT JOIN [{Global.DbViClock}].dbo.tblBoPhan bp ON nv.NVMaBP = bp.BPMa
            //        LEFT JOIN [{Global.DbViClock}].dbo.tblChucVu cv ON nv.NVMaCV = cv.CVMa
            //        {whereSql}
            //        ORDER BY u.Id
            //        OFFSET (@PageNumber - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY
            //    )
            //    SELECT 
            //        pu.Id,
            //        pu.UserCode,
            //        pu.NVHoTen,
            //        pu.NVMaBP,
            //        pu.BPTen,
            //        pu.CVTen,
            //        pu.NVMaCV,
            //        pu.NVGioiTinh,
            //        pu.Phone,
            //        pu.Email,
            //        pu.NVNgayVao,
            //        pu.DateOfBirth
            //    FROM PagedUsers pu
            //");

            //var users = await _viclockDapperContext.QueryAsync<GetAllUserResponse>(dataSql.ToString(), param);

            //var results = users
            //    .GroupBy(x => x.Id)
            //    .Select(g => new GetAllUserResponse
            //    {
            //        Id = g.First().Id,
            //        UserCode = g.First().UserCode,
            //        NVHoTen = g.First().NVHoTen,
            //        NVMaBP = g.First().NVMaBP,
            //        BPTen = g.First().BPTen,
            //        CVTen = g.First().CVTen,
            //        NVMaCV = g.First().NVMaCV,
            //        NVGioiTinh = g.First().NVGioiTinh,
            //        Phone = g.First().Phone,
            //        Email = g.First().Email,
            //        DateOfBirth = g.First().DateOfBirth,
            //        NVNgayVao = g.First().NVNgayVao,
            //    })
            //    .ToList();

            //return new PagedResults<GetAllUserResponse>
            //{
            //    Data = results,
            //    TotalItems = totalItems,
            //    TotalPages = totalPages
            //};
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
                        "dbo.GetUserInfoBetweenWebSystemAndViclock",
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

            var result = await connection.QueryFirstAsync<object>(sql, new { UserCode = userCode });

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
        public async Task<List<GetMultiUserViClockByOrgUnitIdResponse>> GetMultipleUserViclockByOrgUnitId(int OrgUnitId)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"SELECT
                            NVMa,
                            NVMaNV,
                            {Global.DbViClock}.dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen,
                            OrgUnitID,
	                        COALESCE(NULLIF(Email, ''), NVEmail, '') AS Email
                        FROM {Global.DbViClock}.[dbo].[tblNhanVien] AS NV
                        RIGHT JOIN {Global.DbWeb}.dbo.users as U
	                        ON NV.NVMaNV = U.UserCode
                        WHERE
                            NV.OrgUnitID = @OrgUnitId AND NV.NVNgayRa > GETDATE()";

            var result = await connection.QueryAsync<GetMultiUserViClockByOrgUnitIdResponse>(sql, new { OrgUnitID = OrgUnitId });

            return (List<GetMultiUserViClockByOrgUnitIdResponse>)result;
        }

        //xây dựng hierarchy sơ đồ tổ chức
        public async Task<List<OrgUnitNode>> BuildOrgTree(int departmentId)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql1 = $@"SELECT TOP (1000) OU.[Id] AS OrgUnitId
                          ,OU.[DeptId]
                          ,OU.[Name] AS OrgUnitName
                          ,OU.[UnitId]
                          ,OU.[ParentOrgUnitId]
                          ,OU.[ParentJobTitleId],
                       NV.NVMaNV,
                       vs_new.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
                       OU1.Name
                      FROM [ServicePortal].[dbo].[org_units] AS OU
                      LEFT JOIN vs_new.dbo.tblNhanVien AS NV ON NV.NVNgayRa > GETDATE() AND NV.OrgUnitID = OU.Id
                      LEFT JOIN [ServicePortal].[dbo].[org_units] AS OU1 ON OU.ParentOrgUnitId = OU1.Id
                      WHERE OU.DeptId = @departmentId
                      AND OU.UnitId >= 5
                      ORDER BY UnitId ASC, ParentOrgUnitId ASC";

            var result = await connection.QueryAsync<OrgUnitNode>(sql1, new { departmentId });
            var orgUnits = result.ToList();

            var groupedByOrgUnitId = orgUnits?.GroupBy(x => x.OrgUnitId)?.ToDictionary(g => g.Key, g => g.ToList());
            var dict = orgUnits?.GroupBy(x => x.OrgUnitId)?.ToDictionary(g => g.Key, g => g.First());

            List<OrgUnitNode> roots = new List<OrgUnitNode>();

            foreach (var node in orgUnits)
            {
                if (node.ParentJobTitleId.HasValue)
                {
                    if (dict.TryGetValue(node.ParentJobTitleId.Value, out var parent))
                    {
                        parent.Children.Add(node);
                    }
                    else
                    {
                        roots.Add(node);
                    }
                }
                else
                {
                    roots.Add(node);
                }
            }

            return roots;
        }

        //lấy thông tin của người quản lý tiếp theo
        public async Task<dynamic?> GetUserByParentOrgUnit(int parentOrgUnitId)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sql = $@"
                SELECT NV.NVMaNV, [{Global.DbViClock}].dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen
                FROM [{Global.DbWeb}].dbo.org_units AS OG
                INNER JOIN [{Global.DbViClock}].dbo.tblNhanVien AS NV On OG.Id = NV.OrgUnitID
                WHERE OG.ParentOrgUnitId = @ParentOrgUnitId
            ";

            var param = new
            {
                ParentOrgUnitId = parentOrgUnitId,
            };

            var data = await connection.QueryAsync<dynamic>(sql, param);

            return data;
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

        //lấy thông tin người tiếp theo approval theo usercode
        public async Task<List<NextUserInfoApprovalResponse>> GetNextUserInfoApprovalByCurrentUserCode(string userCode)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                WITH CurrentOrgUnitUser AS (
                    SELECT
		                NV.OrgUnitID
	                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
	                WHERE NV.NVMaNV = @userCode
                ),
                NextOrgUnitUser AS (
	                SELECT 
		                ORG.ParentJobTitleId
	                FROM {Global.DbWeb}.dbo.org_units AS ORG
	                INNER JOIN CurrentOrgUnitUser AS CORG ON CORG.OrgUnitID = ORG.Id
                )
                SELECT
	                NV.NVMaNV,
	                {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
	                BP.BPTen,
	                CV.CVTen,
                    COALESCE(U.Email, NV.NVEmail, '') AS Email,
	                NV.OrgUnitID
                FROM NextOrgUnitUser AS NORG
                INNER JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV ON NORG.ParentJobTitleId = NV.OrgUnitID
                LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
                LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan AS BP ON NV.NVMaBP = BP.BPMa
                LEFT JOIN {Global.DbViClock}.dbo.tblChucVu AS CV On NV.NVMaCV = CV.CVMa
            ";

            var result = await connection.QueryAsync<NextUserInfoApprovalResponse>(sql, new { userCode });

            return (List<NextUserInfoApprovalResponse>)result;
        }
    }
}
