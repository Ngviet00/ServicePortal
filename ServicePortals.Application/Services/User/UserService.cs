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
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Hubs;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly ICacheService _cacheService;

        public UserService(
            ApplicationDbContext context,
            NotificationService notificationService,
            IViclockDapperContext viclockDapperContext,
            ICacheService cacheService
        )
        {
            _context = context;
            _viclockDapperContext = viclockDapperContext;
            _cacheService = cacheService;
        }

        //lấy danh sách user, kết hợp vs tblNhanVien bên db viclock
        public async Task<PagedResults<GetAllUserResponse>> GetAll(GetAllUserRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;
            string? DepartmentName = request.DepartmentName;
            int? Sex = request.Sex;
            int? PositionId = request.PositionId;

            var param = new
            {
                SearchName = Helper.RemoveDiacritics(name),
                SearchDept = DepartmentName,
                SearchSex = Sex,
                SearchPosition = PositionId,
                PageNumber = (int)page,
                PageSize = (int)pageSize
            };

            var whereSql = new StringBuilder();

            whereSql.AppendLine(" WHERE 1=1 AND nv.NVNgayRa > GETDATE()");

            if (!string.IsNullOrWhiteSpace(name))
            {
                whereSql.AppendLine($" AND (dbo.udf_RemoveDiacritics(dbo.funTCVN2Unicode(nv.NVHoTen)) LIKE '%' + @SearchName + '%' ");
                whereSql.AppendLine($" OR u.UserCode LIKE '%' + @SearchName + '%' )");
            }

            if (DepartmentName != null)
            {
                whereSql.AppendLine($" AND bp.BPTen = @SearchDept ");
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
                LEFT JOIN [{Global.DbViClock}].dbo.tblBoPhan bp ON nv.NVMaBP = bp.BPMa
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
                        u.Phone,
                        u.Email,
                        u.DateOfBirth,
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
                    pu.Phone,
                    pu.Email,
                    pu.NVNgayVao,
                    pu.DateOfBirth
                FROM PagedUsers pu
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
                    Phone = g.First().Phone,
                    Email = g.First().Email,
                    DateOfBirth = g.First().DateOfBirth,
                    NVNgayVao = g.First().NVNgayVao,
                })
                .ToList();

            return new PagedResults<GetAllUserResponse>
            {
                Data = results,
                TotalItems = totalItems,
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
                var info = await _context.Database.GetDbConnection()
                    .QueryFirstOrDefaultAsync<PersonalInfoResponse>(
                        "dbo.GetUserInfoBetweenWebSystemAndViclock",
                        new { userCode },
                        commandType: CommandType.StoredProcedure
                    );

                var roleAndPermissionUser = await GetRoleAndPermissionByUser(userCode);
                var formatRoleAndPermission = FormatRoleAndPermissionByUser(roleAndPermissionUser);

                if (info != null)
                {
                    info.Roles = formatRoleAndPermission.Roles;
                    info.Permissions = formatRoleAndPermission.Permissions;
                }

                return info;
            }, expireMinutes: 2);

            return result;
        }

        //lấy thông tin user theo mã nhân viên ở db viclock, có thể tùy chỉnh thêm cột nếu muốn
        public async Task<object?> GetCustomColumnUserViclockByUserCode(string userCode, string columns)
        {
            var sql = $@"SELECT
                            NVMa,
                            NVMaNV,
                            [{Global.DbViClock}].dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen,
                            {columns}
                        FROM [{Global.DbViClock}].[dbo].[tblNhanVien]
                        WHERE
                            NVMaNV = @UserCode";

            return await _viclockDapperContext.QueryFirstOrDefaultAsync<object?>(sql, new { UserCode = userCode });
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

        //public static List<OrgUnitNode> BuildOrgUnitTree(List<RawDataRow> flatData)
        //{
        //    // Bước 1: Tạo một map cho tất cả các node chức danh/đơn vị duy nhất.
        //    // Key là Id của chức danh (int), Value là đối tượng OrgUnitNode đại diện cho chức danh đó.
        //    var jobTitleNodesMap = new Dictionary<int, OrgUnitNode>();

        //    // Duyệt qua dữ liệu phẳng để khởi tạo các node chức danh và tách riêng các người dùng.
        //    foreach (var row in flatData)
        //    {
        //        // Nếu Id chức danh chưa có trong map, tạo một node mới cho chức danh đó.
        //        if (!jobTitleNodesMap.ContainsKey(row.Id))
        //        {
        //            jobTitleNodesMap[row.Id] = new OrgUnitNode
        //            {
        //                OrgUnitId = row.Id,
        //                OrgUnitName = row.Name, // Tên chức danh từ cột 'Name' đầu tiên
        //                ParentJobTitleId = row.ParentJobTitleId,
        //                // Các thông tin NVMaNV, NVHoTen sẽ được xử lý sau nếu có người kèm theo chức danh này
        //                AssociatedOrgUnitName = row.OrgUnitName // Cột 'Name' cuối cùng
        //            };
        //        }
        //    }

        //    // Bước 2: Gắn người dùng vào các node chức danh tương ứng và thiết lập quan hệ cha-con cho các chức danh.
        //    var rootNodes = new List<OrgUnitNode>();

        //    // Lặp qua dữ liệu phẳng một lần nữa để xử lý người dùng và xây dựng cây.
        //    foreach (var row in flatData)
        //    {
        //        // Lấy node chức danh hiện tại từ map (chắc chắn tồn tại sau Bước 1)
        //        var currentJobTitleNode = jobTitleNodesMap[row.Id];

        //        // Nếu hàng này có thông tin người dùng, tạo một node con cho người đó.
        //        if (!string.IsNullOrEmpty(row.NVMaNV))
        //        {
        //            var personNode = new OrgUnitNode
        //            {
        //                OrgUnitId = null, // Node người dùng không có OrgUnitId riêng
        //                OrgUnitName = row.NVHoTen, // Tên của node người dùng là tên người
        //                NVMaNV = row.NVMaNV,
        //                NVHoTen = row.NVHoTen,
        //                ParentJobTitleId = row.Id, // Cha của người dùng là chức danh hiện tại
        //                AssociatedOrgUnitName = row.OrgUnitName // Có thể giữ thông tin này cho người dùng
        //            };
        //            currentJobTitleNode.Children.Add(personNode);
        //        }
        //    }

        //    // Bước 3: Thiết lập quan hệ cha-con giữa các chức danh và xác định các node gốc.
        //    foreach (var node in jobTitleNodesMap.Values)
        //    {
        //        // Nếu ParentJobTitleId tồn tại và tìm thấy cha trong map
        //        if (node.ParentJobTitleId.HasValue && jobTitleNodesMap.ContainsKey(node.ParentJobTitleId.Value))
        //        {
        //            var parentNode = jobTitleNodesMap[node.ParentJobTitleId.Value];
        //            parentNode.Children.Add(node);
        //        }
        //        else
        //        {
        //            // Nếu ParentJobTitleId là NULL hoặc không tìm thấy cha hợp lệ, đây là một node gốc.
        //            rootNodes.Add(node);
        //        }
        //    }

        //    return rootNodes;
        //}

        //public static List<OrgUnitNode_1> BuildOrgChartTree(List<OrgUnitNode_1> flatList)
        //{
        //    var lookup = flatList.ToDictionary(x => x.UnitId!.Value, x => x);
        //    var roots = new List<OrgUnitNode_1>();

        //    foreach (var node in flatList)
        //    {
        //        if (node.ParentOrgUnitId.HasValue && lookup.ContainsKey(node.ParentOrgUnitId.Value))
        //        {
        //            var parent = lookup[node.ParentOrgUnitId.Value];
        //            parent.Children.Add(node);
        //        }
        //        else
        //        {
        //            // Không có cha → gốc
        //            roots.Add(node);
        //        }
        //    }

        //    return roots;
        //}

        //xây dựng hierarchy sơ đồ tổ chức
        public async Task<List<OrgUnitNode_1>> BuildOrgTree(int departmentId)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql1 = $@"SELECT TOP (1000) OU.[Id]
                          ,OU.[DeptId]
                          ,OU.[Name]
                          ,OU.[UnitId]
                          ,OU.[ParentOrgUnitId]
                          ,OU.[ParentJobTitleId],
	                      NV.NVMaNV,
	                      NV.NVHoTen,
	                      OU1.Name
                      FROM [ServicePortal].[dbo].[org_units] AS OU
                      LEFT JOIN vs_new.dbo.tblNhanVien AS NV ON NV.NVNgayRa > GETDATE() AND NV.OrgUnitID = OU.Id
                      LEFT JOIN [ServicePortal].[dbo].[org_units] AS OU1 ON OU.ParentOrgUnitId = OU1.Id
                      WHERE OU.DeptId = 113
                      AND OU.UnitId >= 5
                      ORDER BY UnitId ASC, ParentOrgUnitId ASC";

            var result = await connection.QueryAsync<OrgUnitNode_1>(sql1);
            var orgUnits = result.ToList();

            List<OrgUnitNode_1> results = new List<OrgUnitNode_1>();

            var rootNode = orgUnits.FirstOrDefault(e => e.ParentJobTitleId == null || e.ParentOrgUnitId == null) ?? orgUnits.FirstOrDefault();
            


            var jobTitleNodesMap = new Dictionary<int, OrgUnitNode_1>();

            //foreach (var row in orgUnits)
            //{
            //    // Nếu Id chức danh chưa có trong map, tạo một node mới cho chức danh đó.
            //    if (!jobTitleNodesMap.ContainsKey(row.Id))
            //    {
            //        jobTitleNodesMap[row.Id] = new OrgUnitNode
            //        {
            //            OrgUnitId = row.Id,
            //            OrgUnitName = row.Name, // Tên chức danh từ cột 'Name' đầu tiên
            //            ParentJobTitleId = row.ParentJobTitleId,
            //            // Các thông tin NVMaNV, NVHoTen sẽ được xử lý sau nếu có người kèm theo chức danh này
            //            AssociatedOrgUnitName = row.OrgUnitName // Cột 'Name' cuối cùng
            //        };
            //    }
            //}

            return null;

            //return null;

            //var connection = (SqlConnection)_context.CreateConnection();

            //if (connection.State != ConnectionState.Open)
            //{
            //    await connection.OpenAsync();
            //}

            //var sql = $@"
            //    DECLARE @departmentId_param int = @departmentId;

            //    WITH OrgTree AS (
            //        SELECT *
            //        FROM {Global.DbWeb}.dbo.org_units
            //        WHERE DeptId = @departmentId_param
            //        UNION ALL
            //        SELECT child.*
            //        FROM {Global.DbWeb}.dbo.org_units child
            //        JOIN OrgTree parent ON child.ParentOrgUnitId = parent.Id
            //    ),
            //    MainUsers AS (
            //        SELECT
            //            ou.Id AS OrgUnitId,
            //            ou.Name AS OrgUnitName,
            //            ou.ParentJobTitleId,
            //            u.NVMaNV AS NVMaNV,
            //            u.NVHoTen
            //        FROM OrgTree ou
            //        LEFT JOIN {Global.DbViClock}.dbo.tblNhanVien u ON u.OrgUnitId = ou.Id
            //        WHERE u.NVMaNV IS NOT NULL
            //    ),
            //    AddMoreUser AS (
            //        SELECT DISTINCT
            //            ou.Id AS OrgUnitId,
            //            ou.Name AS OrgUnitName,
            //            CAST(NULL AS INT) AS ParentJobTitleId,
            //            u.NVMaNV AS NVMaNV,
            //            u.NVHoTen
            //        FROM {Global.DbViClock}.dbo.tblNhanVien u
            //        JOIN {Global.DbWeb}.dbo.org_units ou ON ou.Id = u.OrgUnitId
            //        WHERE ou.Id IN (
            //            SELECT DISTINCT ParentJobTitleId FROM OrgTree
            //        )
            //        AND u.NVMaNV NOT IN (SELECT NVMaNV FROM MainUsers WHERE NVMaNV IS NOT NULL) AND u.NVMaNV IS NOT NULL
            //    ),
            //    CombinedUsers AS (
            //     SELECT 
            //      OrgUnitId,
            //      OrgUnitName,
            //      ParentJobTitleId,
            //      NVMaNV,
            //      {Global.DbViClock}.dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen
            //     FROM MainUsers
            //     UNION
            //     SELECT 
            //      OrgUnitId,
            //      OrgUnitName,
            //      ParentJobTitleId,
            //      NVMaNV,
            //      {Global.DbViClock}.dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen
            //     FROM AddMoreUser
            //    )
            //    SELECT CU.* FROM CombinedUsers AS CU
            //    LEFT JOIN {Global.DbWeb}.dbo.users AS U
            //    on CU.NVMaNV = u.UserCode
            //    ORDER BY CU.OrgUnitId ASC
            //";

            //var result = await connection.QueryAsync<OrgUnitNode>(sql, new { departmentId });
            //var orgUnits = result.ToList();

            //var groupedByOrgUnitId = orgUnits?.GroupBy(x => x.OrgUnitId)?.ToDictionary(g => g.Key, g => g.ToList());
            //var dict = orgUnits?.GroupBy(x => x.OrgUnitId)?.ToDictionary(g => g.Key, g => g.First());

            //List<OrgUnitNode> roots = new List<OrgUnitNode>();

            //foreach (var node in orgUnits)
            //{
            //    if (node.ParentJobTitleId.HasValue)
            //    {
            //        if (dict.TryGetValue(node.ParentJobTitleId.Value, out var parent))
            //        {
            //            parent.Children.Add(node);
            //            node.ParentName = parent.OrgUnitName;
            //        }
            //        else
            //        {
            //            node.ParentName = "Không có cha";
            //            roots.Add(node);
            //        }
            //    }
            //    else
            //    {
            //        node.ParentName = "Không có cha";
            //        roots.Add(node);
            //    }
            //}

            //return roots;
        }

        //lấy thông tin của người quản lý tiếp theo
        public async Task<dynamic?> GetUserByParentOrgUnit(int parentOrgUnitId)
        {
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

            var data = await _viclockDapperContext.QueryAsync<dynamic>(sql, param);

            return data;
        }

        //tìm kiếm tất cả người dùng ở bên viclock
        public async Task<PagedResults<object>> SearchAllUserFromViClock(SearchAllUserFromViclockRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

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
                sbWhere.AppendLine($" AND (dbo.udf_RemoveDiacritics(dbo.funTCVN2Unicode(NV.NVHoTen)) LIKE '%' + @Key + '%' ");
                sbWhere.AppendLine($" OR NV.NVMaNV LIKE '%' + @Key + '%' )");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT");
            sb.AppendLine("     NV.NVMaNV,");
            sb.AppendLine("     dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen,");
            sb.AppendLine("     BP.BPMa,");
            sb.AppendLine("     dbo.funTCVN2Unicode(BP.BPTen) as BPTen,");
            sb.AppendLine("     NV.NVNgayVao");
            sb.AppendLine(" FROM tblNhanVien as NV");

            sb.AppendLine(" INNER JOIN tblBoPhan as BP");
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
            countSql.AppendLine(" FROM tblNhanVien AS NV");
            countSql.AppendLine(" INNER JOIN tblBoPhan as BP ON NV.NVMaBP = BP.BPMa");
            countSql.AppendLine($"{sbWhere}");

            int totalItems = await _viclockDapperContext.QueryFirstOrDefaultAsync<int>(countSql.ToString(), param);

            int totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var data = await _viclockDapperContext.QueryAsync<object>(sb.ToString(), param);

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
        public async Task<dynamic> Test()
        {
            return null;
        }
    }
}
