using System.Text;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.HRManagement.DTO.Requests;
using ServicePortal.Common;
using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Cache;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Persistence;
using ServicePortal.Modules.HRManagement.DTO;

namespace ServicePortal.Applications.Modules.HRManagement.Services
{
    public class HRManagementService : IHRManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDapperQueryService _dapperQueryService;
        private readonly ICacheService _cacheService;

        public HRManagementService(
            ApplicationDbContext context, 
            IDapperQueryService dapperQueryService,
            ICacheService cacheService
        )
        {
            _context = context;
            _dapperQueryService = dapperQueryService;
            _cacheService = cacheService;
        }

        public async Task<object> GetAllHR()
        {
            var result = await _cacheService.GetOrCreateAsync($"list_hr", async () =>
            {
                var sql = $@"
                    SELECT 
                        NVMaNV,
                        [{Global.DbViClock}].[dbo].funTCVN2Unicode([NVHoTen]) as [NVHoTen]
                    FROM [{Global.DbViClock}].[dbo].[tblNhanVien]
                    WHERE NVMaBP = @NVMaBP AND NVNgayRa > GETDATE()
                ";

                var param = new
                {
                    NVMaBP = 110
                };

                var data = await _dapperQueryService.QueryAsync<object>(sql, param);

                return data;

            }, expireMinutes: 1440);

            return result;
        }

        public async Task<object> SaveHrManagement(SaveHRManagementRequest request)
        {
           const string MANAGE_TIMEKEEPING = "MANAGE_TIMEKEEPING";
           const string MANAGE_TRAINING = "MANAGE_TRAINING";
           const string MANAGE_RECRUITMENT = "MANAGE_RECRUITMENT";

            //delete
            var userTypePairs = new List<(string? UserCode, string Type)>();

            userTypePairs.AddRange(request.ManageTimekeeping?.Select(x => (x.Value, MANAGE_TIMEKEEPING)) ?? []);
            userTypePairs.AddRange(request.ManageTraining?.Select(x => (x.Value, MANAGE_TRAINING)) ?? []);
            userTypePairs.AddRange(request.ManageRecruitment?.Select(x => (x.Value, MANAGE_RECRUITMENT)) ?? []);

            var oldRecords = await _context.HrManagements
                .Where(x => userTypePairs.Select(p => p.UserCode).Contains(x.UserCode)
                         && userTypePairs.Select(p => p.Type).Contains(x.Type))
                .ToListAsync();

            oldRecords = oldRecords
                .Where(x => userTypePairs.Any(p => p.UserCode == x.UserCode && p.Type == x.Type))
                .ToList();

            _context.HrManagements.RemoveRange(oldRecords);

            //add new
            var hrManagements = new List<HrManagements>();

            if (request.ManageTimekeeping != null && request.ManageTimekeeping.Count() > 0)
            {
                foreach (var item in request.ManageTimekeeping)
                {
                    hrManagements.Add(new HrManagements
                    {
                        Type = MANAGE_TIMEKEEPING,
                        UserCode = item.Value,
                        UserName = item.Label,
                    });
                }
            }

            if (request.ManageTraining != null && request.ManageTraining.Count() > 0)
            {
                foreach (var item in request.ManageTraining)
                {
                    hrManagements.Add(new HrManagements
                    {
                        Type = MANAGE_TRAINING,
                        UserCode = item.Value,
                        UserName = item.Label,
                    });
                }
            }

            if (request.ManageRecruitment != null && request.ManageRecruitment.Count() > 0)
            {
                foreach (var item in request.ManageRecruitment)
                {
                    hrManagements.Add(new HrManagements
                    {
                        Type = MANAGE_RECRUITMENT,
                        UserCode = item.Value,
                        UserName = item.Label,
                    });
                }
            }

            _context.HrManagements.AddRange(hrManagements);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> GetUsersWithAttendanceManagers()
        {
            var items = await _cacheService.GetOrCreateAsync(Global.CacheKeyGetAllUserManageAttendance, async () =>
            {
                return await _context.UserManageAttendances.ToListAsync();

            }, expireMinutes: 1440);

            return items;
        }

        public async Task<object> AssignMultiplePeopleToAttendanceManager(AssignMultiplePeopleToAttendanceManagerRequest request)
        {
            if (request.UserCodesManage != null && request.UserCodes != null)
            {
                var existing = await _context.UserManageAttendanceUsers
                    .Where(x => request.UserCodes.Contains(x.UserCode ?? ""))
                    .ToListAsync();

                var toDelete = existing
                    .Where(x => !request.UserCodesManage.Contains(x.UserCodeManage ?? "") && request.UserCodes.Contains(x.UserCode ?? ""))
                    .ToList();

                _context.UserManageAttendanceUsers.RemoveRange(toDelete);

                var userManageAttendanceUsers = new List<UserManageAttendanceUser>();

                foreach (var manager in request.UserCodesManage)
                {
                    foreach (var user in request.UserCodes)
                    {
                        bool exists = existing.Any(x => x.UserCodeManage == manager && x.UserCode == user);
                        if (!exists)
                        {
                            userManageAttendanceUsers.Add(new UserManageAttendanceUser
                            {
                                UserCodeManage = manager,
                                UserCode = user
                            });
                        }
                    }
                }

                _context.UserManageAttendanceUsers.AddRange(userManageAttendanceUsers);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<object> HrAssignAttendanceManagers(HrAssignAttendanceManagersRequest request)
        {
            if (request.Data != null)
            {
                var userManageAttendances = await _context.UserManageAttendances.ToListAsync();

                var userFromClientDict = request.Data.ToDictionary(u => u.UserCode, u => u);

                var userManageAttendancesDict = userManageAttendances.ToDictionary(u => u.UserCode, u => u);

                var usersToDelete = userManageAttendances.Where(dbUser => userFromClientDict != null && !userFromClientDict.ContainsKey(dbUser.UserCode)).ToList();

                _context.UserManageAttendances.RemoveRange(usersToDelete);

                var usersToAdd = new List<UserManageAttendance>();

                foreach (var item in request.Data ?? [])
                {
                    if (!userManageAttendancesDict.TryGetValue(item.UserCode, out var dbUser))
                    {
                        usersToAdd.Add(new UserManageAttendance
                        {
                            UserCode = item.UserCode,
                            UserName = item.UserName,
                        });
                    }
                }

                _context.UserManageAttendances.AddRange(usersToAdd);

                await _context.SaveChangesAsync();

                _cacheService.Remove(Global.CacheKeyGetAllUserManageAttendance);

                return true;
            }

            throw new Exception("Server Error");
        }

        public async Task<PagedResults<object>> GetAssignableAttendanceUsersRequest(GetAssignableAttendanceUsersRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            var param = new
            {
                Key = Helper.UnicodeToTCVN(request.Key ?? ""),
                request.DepartmentId,
                request.Page,
                request.PageSize,
            };

            StringBuilder sbWhere = new();

            sbWhere.AppendLine(" WHERE 1=1 AND NV.NVNgayRa > GETDATE()");

            if (!string.IsNullOrWhiteSpace(request.Key))
            {
                sbWhere.AppendLine(" AND (NV.NVHoTen like '%' + @Key + '%' OR NV.NVMaNV = @Key)");
            }

            if (request.DepartmentId != null && request.DepartmentId != 0)
            {
                sbWhere.AppendLine(" AND BP.BPMa = @DepartmentId");
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" SELECT");
            sb.AppendLine("     NV.NVMaNV,");
            sb.AppendLine("     dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen,");
            sb.AppendLine("     BP.BPMa,");
            sb.AppendLine("     dbo.funTCVN2Unicode(BP.BPTen) as BPTen,");
            sb.AppendLine("     NV.NVNgayVao,");
            sb.AppendLine("     STRING_AGG(UMA.UserName, ', ') AS UserNamesManageAttendance");
            sb.AppendLine(" FROM tblNhanVien as NV");

            sb.AppendLine(" INNER JOIN tblBoPhan as BP");
            sb.AppendLine(" ON NV.NVMaBP = BP.BPMa");

            sb.AppendLine($@" LEFT JOIN [{Global.DbWeb}].dbo.user_manage_attendance_users as UMAU ON NV.NVMaNV = UMAU.UserCode");
            sb.AppendLine($@" LEFT JOIN [{Global.DbWeb}].dbo.user_manage_attendances as UMA ON UMAU.UserCodeManage = UMA.UserCode");

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

            int totalItems = await _dapperQueryService.QueryFirstOrDefaultAsync<int>(countSql.ToString(), param);

            int totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var data = await _dapperQueryService.QueryAsync<object>(sb.ToString(), param);

            return new PagedResults<object>
            {
                Data = (List<object>)data,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<object> ChangeManageAttendance(ChangeManageAttendanceRequest request)
        {
            var newUserManage = request.NewUserManageAttendance;
            var oldUserManage = request.OldUserManageAttendance;

            if (newUserManage != null && oldUserManage != null)
            {
                if (newUserManage.Value == oldUserManage.Value)
                {
                    throw new ValidationException("Người cũ và người mới phải khác nhau");
                }

                await _context.UserManageAttendanceUsers
                    .Where(e => e.UserCodeManage == oldUserManage.Value)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.UserCodeManage, x => newUserManage.Value));

                return true;
            }

            throw new BadRequestException("Data error");
        }

        public async Task<List<HrManagements>> GetHrManagements()
        {
            return await _context.HrManagements.ToListAsync();
        }

        public async Task<List<HrManagements>> GetHrManagementsByType(string type)
        {
            return await _context.HrManagements.Where(x => x.Type == type).ToListAsync();
        }

        public async Task<List<string>> GetEmailHRByType(string type)
        {
            var result = await _context.Users
                .Join(
                    _context.HrManagements,
                    user => user.UserCode,
                    hr => hr.UserCode,
                    (user, hr) => new { User = user, HrManagements = hr}
                )
                .Where(joined => joined.HrManagements.Type == type)
                .Select(joined => joined.User.Email)
                .ToListAsync();

            return result;
        }
    }
}
