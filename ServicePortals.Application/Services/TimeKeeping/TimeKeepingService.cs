using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.TimeKeeping;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using ServicePortals.Shared.SharedDto;

namespace ServicePortals.Infrastructure.Services.TimeKeeping
{
    public class TimeKeepingService : ITimeKeepingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly IEmailService _emailService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ExcelService _excelService;

        public TimeKeepingService (
            ExcelService excelService,
            IViclockDapperContext viclockDapperContext, 
            IEmailService emailService, 
            ApplicationDbContext context,
            ILeaveRequestService leaveRequestService
        )
        {
            _viclockDapperContext = viclockDapperContext;
            _emailService = emailService;
            _context = context;
            _leaveRequestService = leaveRequestService;
            _excelService = excelService;
        }

        //lấy sanh sách chấm công của user, tìm kiếm theo tháng, param truyền vào gồm fromdate và to date, usercode
        public async Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingRequest request)
        {
            var param = new
            {
                request.FromDate,
                request.ToDate,
                StaffCode = request.UserCode
            };

            var result = await _viclockDapperContext.QueryAsync<object>(
                "[dbo].[cp_get_table_timekeeping]",
                param,
                CommandType.StoredProcedure
            );

            return result;
        }

        //màn quản lý chấm công, người quản lý chấm công quản lý chấm công của người khác
        public async Task<PagedResults<GroupedUserTimeKeeping>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request)
        {
            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            int month = request.Month ?? DateTime.UtcNow.Month;
            int year = request.Year ?? DateTime.UtcNow.Year;

            int dayInMonth = DateTime.DaysInMonth(year, month);

            string tblName = $"{Global.DbViClock}.dbo.BaoCaoVS5{year}_{month:D2}";

            bool isTableExists = connection.ExecuteScalar<int?>("SELECT OBJECT_ID(@tableName, 'U')", new { tableName = tblName }).HasValue;

            if (!isTableExists)
            {
                return new PagedResults<GroupedUserTimeKeeping>
                {
                    Data = [],
                    TotalItems = 0,
                    TotalPages = 0
                };
            }

            double pageSize = request.PageSize > 0 ? request.PageSize : 10;
            double page = request.Page > 0 ? request.Page : 1;
            string keySearch = request.keySearch?.Trim() ?? "";

            int? deptId = request.DeptId;
            int? team = request.Team;

            var fromDate = $"{year}-{month:D2}-01";
            var toDate = $"{year}-{month:D2}-{dayInMonth}";

            string sqlOrgUnitIdQuery = $@"
                WITH RecursiveOrg AS (
                    SELECT o.id FROM user_mng_org_unit_id as um
                    INNER JOIN [{Global.DbWeb}].dbo.org_units as o
                        ON um.OrgUnitId = o.id
                    WHERE um.UserCode = @userCode AND um.ManagementType = @type
            ";

            if (deptId == null)
            {
                sqlOrgUnitIdQuery += $@"
                    UNION ALL
                    SELECT o.id FROM  {Global.DbWeb}.dbo.org_units o INNER JOIN RecursiveOrg ro ON o.ParentOrgUnitId = ro.id
                )
                SELECT DISTINCT id FROM RecursiveOrg
                ";
            }
            else
            {
                sqlOrgUnitIdQuery += $@"
                )
                SELECT 
                    o.id 
                    FROM 
                    {Global.DbWeb}.dbo.org_units o 
                    INNER JOIN RecursiveOrg ro ON o.ParentOrgUnitId = ro.id AND o.DeptId = @deptId
                ";
            }

            var orgUnitIds = (await connection.QueryAsync<int>(sqlOrgUnitIdQuery, new
            {
                userCode = request.UserCode,
                type = "MNG_TIME_KEEPING",
                deptId
            })).ToList();

            if (orgUnitIds.Count == 0)
            {
                return new PagedResults<GroupedUserTimeKeeping>
                {
                    Data = [],
                    TotalItems = 0,
                    TotalPages = 0
                };
            }

            var countUser = await connection.QuerySingleAsync<int>(
                @"SELECT COUNT(*) FROM vs_new.dbo.tblNhanVien AS NV WHERE NV.NVNgayRa > GETDATE() AND NV.OrgUnitID IN @ids AND (@KeySearch = '' OR NV.NVMaNV = @KeySearch)",
                new { ids = orgUnitIds, KeySearch = keySearch }
            );

            var totalPages = (int)Math.Ceiling((double)countUser / pageSize);

            var sql = $@"
                WITH Users AS (
                    SELECT
                        NV.NVMa, NV.NVMaNV, {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen, BP.BPTen
                    FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                    LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan AS BP ON NV.NVMaBP = BP.BPMa AND NV.NVNgayRa > GETDATE() AND NV.OrgUnitID IS NOT NULL
                    WHERE NV.OrgUnitID IN @ids
                        AND (@KeySearch = '' OR NV.NVMaNV = @KeySearch)
                    ORDER BY NV.NVMa ASC
                    OFFSET (@Page - 1) * @PageSize ROWS
                    FETCH NEXT @PageSize ROWS ONLY
                )
                SELECT 
	                U.NVMaNV,
	                U.NVHoTen,
	                U.BPTen,
	                CASE 
                        WHEN DATEPART(dw, BCNGay) = 1 THEN 'CN' 
                        ELSE convert(nvarchar(10),DATEPART(dw, BCNGay)) 
                    END AS Thu,
	                CONVERT(VARCHAR(10), BC.BCNgay, 120) AS BCNgay,
	                BC.BCTGDen,
	                BC.BCTGVe,
                    BC.BCGhiChu,
	                CASE
		                --Chủ nhật
		                WHEN DATEPART(dw, BCNGay) = 1 THEN 
			                CASE
				                WHEN BCTGDen IS NOT NULL AND BCTGVe IS NOT NULL AND BCGhiChu = 'CN' THEN 'CN_X'
                                ELSE 'CN'
			                END

		                -- ngày thường
		                ELSE
			                CASE
                                WHEN BC.BCGhiChu IS NOT NULL AND BC.BCGhiChu != '' THEN BC.BCGhiChu
				                WHEN (BC.BCTGLamNgay + BC.BCTGLamToi) = BC.BCTGQuyDinh THEN 'X'
				                WHEN BC.BCTGDen IS NOT NULL AND BC.BCTGVe IS NOT NULL THEN 
					                CASE 
						                WHEN 1.0 * CEILING(1.0 * (BCTGQuyDinh - (BCTGLamNgay + BCTGLamToi)) / 30.0) * 30 / NULLIF(BCTGQuyDinh, 0) = 1 THEN '?'
						                ELSE RTRIM(FORMAT(1.0 * CEILING(1.0 * (BCTGQuyDinh - (BCTGLamNgay + BCTGLamToi)) / 30.0) * 30 / NULLIF(BCTGQuyDinh, 0),'0.####'))
					                END
				                ELSE
					                '?'
			                END
	                END AS Results
                FROM Users AS U
                LEFT JOIN {Global.DbViClock}.dbo.tblBaoCao AS BC 
                    ON BC.BCMaNV = U.NVMa AND BCNgay BETWEEN @FromDate AND @ToDate AND BC.BCNgay IS NOT NULL 
                WHERE DATEPART(dw, BC.BCNgay) IS NOT NULL
            ";

            if (!string.IsNullOrWhiteSpace(keySearch))
            {
                sql += " AND U.NVMaNV = @KeySearch";
            }

            sql += " ORDER BY U.NVMaNV ASC";

            var param = new
            {
                Page = (int)page,
                PageSize = (int)pageSize,
                Month = month,
                Year = year,
                FromDate = fromDate,
                ToDate = toDate,
                ids = orgUnitIds,
                KeySearch = keySearch
            };

            var result = (await connection.QueryAsync<GetUserTimeKeepingResponse>(sql, param)).ToList();

            var startFindHistory = new DateTimeOffset(year, month, 1, 0, 0, 0, TimeSpan.FromHours(7));
            var endFindHistory = startFindHistory.AddMonths(1);

            //user của những người được chấm công
            var userCodes = result.Select(x => x.NVMaNV).Distinct().ToList();

            var timeAttendanceEditHistories = await _context.TimeAttendanceEditHistories
                .Where(e =>
                    userCodes.Contains(e.UserCode) && 
                    e.Datetime >= startFindHistory && 
                    e.Datetime < endFindHistory)
                .ToListAsync();

            var timeAttendanceDict = timeAttendanceEditHistories
                .ToDictionary(
                    e => (
                        e.UserCode,
                        e.Datetime.HasValue ? e.Datetime.Value.Date : default
                    ),
                    e => new
                    {
                        e.CurrentValue,
                        e.IsSentToHR
                    }
                );

            var groupedResult = result
                .GroupBy(x => new { x.NVMaNV, x.NVHoTen, x.BPTen })
                .Select(g => new GroupedUserTimeKeeping
                {
                    NVMaNV = g.Key.NVMaNV,
                    NVHoTen = g.Key.NVHoTen,
                    BPTen = g.Key.BPTen,
                    DataTimeKeeping = g.Select(x =>
                    {
                        var CustomValueTimeAttendance = timeAttendanceDict.TryGetValue((g.Key.NVMaNV, DateTime.ParseExact(x?.BCNgay ?? "", "yyyy-MM-dd", CultureInfo.InvariantCulture).Date), out var value) ? value : null;
                        
                        return new UserDailyRecord
                        {
                            thu = x.Thu,
                            bcNgay = x.BCNgay,
                            vao = x.BCTGDen,
                            ra = x.BCTGVe,
                            result = x.Results,
                            bcGhiChu = x.BCGhiChu,
                            CustomValueTimeAttendance = CustomValueTimeAttendance?.CurrentValue ?? null,
                            IsSentToHR = CustomValueTimeAttendance?.IsSentToHR ?? false,
                        };
                    }).ToList()
                }).ToList();

            var finalResult = new PagedResults<GroupedUserTimeKeeping>
            {
                Data = groupedResult,
                TotalItems = groupedResult.Count,
                TotalPages = totalPages
            };

            return finalResult;
        }

        //gửi chấm công cho bộ phận HR
        public async Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request)
        {
            int batchSize = 200;

            int month = request.Month ?? DateTime.Now.Month;
            int year = request.Year ?? DateTime.Now.Year;
            int daysInMonth = DateTime.DaysInMonth(year, month);
            string fromDate = $"{year}-{month.ToString("D2")}-01";
            string toDate = $"{year}-{month.ToString("D2")}-{daysInMonth}";

            string userCodeSendConfirm = request.UserCode ?? "";

            var listEditTimeKeeping = await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdate == userCodeSendConfirm && e.IsSentToHR == false).ToListAsync();

            foreach (var item in listEditTimeKeeping)
            {
                item.IsSentToHR = true;
            }

            _context.TimeAttendanceEditHistories.UpdateRange(listEditTimeKeeping);
            await _context.SaveChangesAsync();

            byte[] excelFileEditHistoryTimeKeeping = _excelService.GenerateExcelHistoryEditTimeKeeping(listEditTimeKeeping);

            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sqlCombinedQuery = $@"
                WITH RecursiveOrg AS (
                    SELECT o.id
                    FROM user_mng_org_unit_id as um
                    INNER JOIN {Global.DbWeb}.dbo.org_units as o
                        ON um.OrgUnitId = o.id
                    WHERE um.UserCode = @userCode AND um.ManagementType = @type
                    UNION ALL
                    SELECT o.id
                    FROM {Global.DbWeb}.dbo.org_units o
                    INNER JOIN RecursiveOrg ro ON o.ParentOrgUnitId = ro.id
                ),
                DistinctOrgUnits AS (
                    SELECT DISTINCT id FROM RecursiveOrg
                )
                SELECT
                    NV.NVMa,
                    NV.NVMaNV,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
                    NV.OrgUnitID,
                    BP.BPTen,
                    NV.NVNgayVao
                FROM {Global.DbViClock}.[dbo].[tblNhanVien] AS NV
                LEFT JOIN {Global.DbViClock}.[dbo].[tblBoPhan] AS BP ON BP.BPMa = NV.NVMaBP
                JOIN DistinctOrgUnits DOU ON NV.OrgUnitID = DOU.id
            ";

            var paramCombined = new
            {
                userCode = request.UserCode,
                type = "MNG_TIME_KEEPING",
            };

            var allUsers = await connection.QueryAsync<GetMultiUserViClockByOrgUnitIdConfirmTimeKeepingResponse>(sqlCombinedQuery, paramCombined, commandTimeout: 900);

            int totalUsers = allUsers.Count();
            int totalBatches = (int)Math.Ceiling((double)totalUsers / batchSize);

            using var workbook = new XLWorkbook();
            for (int i = 0; i < totalBatches; i++)
            {
                var currentBatch = allUsers.Skip(i * batchSize).Take(batchSize).ToList();
                bool isFirstBatch = (i == 0);

                var userIds = currentBatch.Select(u => u.NVMa).Distinct().ToList();

                var sqlGetTimeekping = StringSqlGetTimeekping();

                var attendanceLists = (await connection.QueryAsync<GetUserTimeKeepingResponse>(sqlGetTimeekping, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    UserIds = userIds
                }, commandTimeout: 900)).ToList();

                var exportRows = BuildExportRows(currentBatch, attendanceLists, daysInMonth);
                _excelService.GenerateExcelManagerConfirmToHR(workbook, exportRows, isFirstBatch, daysInMonth);
            }

            if (connection.State == ConnectionState.Open)
            {
                await connection.CloseAsync();
            }

            byte[] excelBytes;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                excelBytes = stream.ToArray();
            }

            List<(string FileName, byte[] FileBytes)> attachments =
            [
                ($"BangChamCong_2025-{request.Month:D2}.xlsx", excelBytes),
                ($"DanhSachChinhSua.xlsx", excelFileEditHistoryTimeKeeping)
            ];

            var hrHavePermissionMngLeaveRequest = await _leaveRequestService.GetHrWithManagementLeavePermission();
            var emailUserSend = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == userCodeSendConfirm);
            string bodyMail = $@"Dear HR Team, Please find attached the excel file containing the staff attendance list [{request.Month} - {request.Year}]";

            await _emailService.EmailSendTimeKeepingToHR(
                hrHavePermissionMngLeaveRequest.Select(e => e.Email ?? "").ToList(),
                new List<string> { emailUserSend != null ? emailUserSend.Email : "" },
                $"{request.UserName} - Confirm Time Keeping T{request.Month}-{request.Year}",
                bodyMail,
                attachments,
                false
            );

            return true;
        }

        /// <summary>
        /// Build export rows from users and attendance data
        /// </summary>
        public static List<AttendanceExportRow> BuildExportRows(
            List<GetMultiUserViClockByOrgUnitIdConfirmTimeKeepingResponse> users,
            List<GetUserTimeKeepingResponse> attendances,
            int daysInMonth)
        {
            var exportRows = new List<AttendanceExportRow>();

            foreach (var user in users)
            {
                var row = new AttendanceExportRow
                {
                    UserCode = user.NVMaNV ?? "",
                    FullName = user.NVHoTen ?? "",
                    DepartmentName = user.BPTen ?? "",
                    JoinDate = user.NVNgayVao.HasValue ? user.NVNgayVao.Value.ToString("dd/MM/yyyy") : "",
                    DayValues = new Dictionary<int, string>()
                };

                var userAtt = attendances
                    .Where(a => a.NVMaNV == user.NVMaNV)
                    .ToDictionary(
                        a => DateTime.Parse(a.BCNgay!).Day,
                        a => (a.Results ?? "", a.CustomResult ?? ""));

                for (int day = 1; day <= daysInMonth; day++)
                {
                    if (userAtt.TryGetValue(day, out var val))
                    {
                        var (results, custom) = val;
                        string displayValue = "";

                        if (!string.IsNullOrWhiteSpace(custom))
                        {
                            displayValue = custom;
                        }
                        else if (!string.IsNullOrWhiteSpace(results))
                        {
                            displayValue = results;
                        }

                        row.DayValues[day] = displayValue;
                    }
                    else
                    {
                        row.DayValues[day] = "";
                    }
                }

                exportRows.Add(row);
            }

            return exportRows;
        }

        public string StringSqlGetTimeekping()
        {
            var sql = $@"
                SELECT 
	                BC.BCMaNV,
                    NV.NVMaNV,
                    CASE 
                        WHEN DATEPART(dw, BC.BCNGay) = 1 THEN 'CN' 
                        ELSE convert(nvarchar(10),DATEPART(dw, BC.BCNGay)) 
                    END AS Thu,
                    CONVERT(VARCHAR(10),BC.BCNgay, 120) AS BCNgay,
                    BC.BCTGDen,
                    BC.BCTGVe,
                    BC.BCGhiChu,
                    CASE
                        --Chủ nhật
                        WHEN DATEPART(dw, BC.BCNGay) = 1 THEN 
                            CASE
                                WHEN BC.BCTGDen IS NOT NULL AND BC.BCTGVe IS NOT NULL AND BC.BCGhiChu = 'CN' THEN 'CN_X'
                                ELSE 'CN'
                            END

                        -- ngày thường
                        ELSE
                            CASE
                                WHEN BC.BCGhiChu IS NOT NULL AND BC.BCGhiChu != '' THEN BCGhiChu
                                WHEN (BC.BCTGLamNgay + BC.BCTGLamToi) = BC.BCTGQuyDinh THEN 'X'
                                WHEN BC.BCTGDen IS NOT NULL AND BC.BCTGVe IS NOT NULL THEN 
				                                CASE 
					                                WHEN 1.0 * CEILING(1.0 * (BC.BCTGQuyDinh - (BC.BCTGLamNgay + BC.BCTGLamToi)) / 30.0) * 30 / NULLIF(BC.BCTGQuyDinh, 0) = 1 THEN '?'
					                                ELSE RTRIM(FORMAT(1.0 * CEILING(1.0 * (BC.BCTGQuyDinh - (BC.BCTGLamNgay + BC.BCTGLamToi)) / 30.0) * 30 / NULLIF(BC.BCTGQuyDinh, 0),'0.####'))
				                                END
                                ELSE
				                                '?'
                            END
                    END AS Results,
	                H.CurrentValue AS CustomResult,
	                H.IsSentToHR
                FROM {Global.DbViClock}.dbo.tblBaoCao AS BC
                LEFT JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV ON BC.BCMaNV = NV.NVMa
                LEFT JOIN {Global.DbWeb}.dbo.time_attendance_edit_histories AS H ON BC.BCMaNV = NV.NVMa AND BC.BCNgay = CAST(H.Datetime AS DATE) AND  NV.NVMaNV = H.UserCode
                WHERE DATEPART(dw, BC.BCNgay) IS NOT NULL AND BC.BCNgay BETWEEN @FromDate AND @ToDate AND BC.BCNgay IS NOT NULL
                AND BC.BCMaNV IN @UserIds
            ";

            return sql;
        }

        //Cập nhật mới những người có quyền quản lý chấm công
        public async Task<object> UpdateUserHavePermissionMngTimeKeeping(List<string> userCodes)
        {
            var permissionMngTimekeeping = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "time_keeping.mng_time_keeping");

            if (permissionMngTimekeeping == null)
            {
                throw new Exception("Chưa có quyền quản lý chấm công!");
            }

            var oldUserPermissionsMngTKeeping = await _context.UserPermissions.Where(e => e.PermissionId == permissionMngTimekeeping.Id).ToListAsync();

            _context.UserPermissions.RemoveRange(oldUserPermissionsMngTKeeping);

            List<UserPermission> newUserPermissions = new List<UserPermission>();

            foreach (var code in userCodes)
            {
                newUserPermissions.Add(new UserPermission
                {
                    UserCode = code,
                    PermissionId = permissionMngTimekeeping.Id
                });
            }

            _context.UserPermissions.AddRange(newUserPermissions);

            await _context.SaveChangesAsync();

            return true;
        }

        //Lấy danh sách những người có quyền quản lý chấm công
        public async Task<object> GetUserHavePermissionMngTimeKeeping()
        {
            var permissionMngTimeKeeping = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "time_keeping.mng_time_keeping");

            if (permissionMngTimeKeeping == null)
            {
                throw new NotFoundException("Permission manage time keeping not found");
            }

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                SELECT
                     NV.NVMaNV,
                     {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen,
                     BP.BPMa,
                     {Global.DbViClock}.dbo.funTCVN2Unicode(BP.BPTen) as BPTen
                FROM user_permissions AS UP
                INNER JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV
                INNER JOIN {Global.DbViClock}.dbo.tblBoPhan as BP
                ON NV.NVMaBP = BP.BPMa
                On UP.UserCode = NV.NVMaNV
                WHERE UP.PermissionId = @PermissionId
            ";

            var param = new
            {
                PermissionId = permissionMngTimeKeeping.Id
            };

            var result = await connection.QueryAsync<object>(sql, param);

            return result;
        }

        //Chọn các vị trí được quản lý chấm công theo người dùng, vdu: người a qly tổ c, tổ d, thì ở màn qly chấm công người a sẽ qly chấm công của những người thuộc tổ c, tổ d
        public async Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request)
        {
            var userMngOrgUnitIds = await _context.UserMngOrgUnits
                .Where(e => e.UserCode == request.UserCode && e.ManagementType == "MNG_TIME_KEEPING")
                .ToListAsync();

            var existingIds = userMngOrgUnitIds.Select(e => e.OrgUnitId).ToHashSet();
            var newIds = request.OrgUnitIds.ToHashSet();

            _context.UserMngOrgUnits.RemoveRange(userMngOrgUnitIds.Where(e => !newIds.Contains((int)(e?.OrgUnitId))));

            _context.UserMngOrgUnits.AddRange(request.OrgUnitIds
                .Where(id => !existingIds.Contains(id))
                .Select(id => new UserMngOrgUnitId
                {
                    UserCode = request.UserCode,
                    OrgUnitId = id,
                    ManagementType = "MNG_TIME_KEEPING"
                })
            );

            await _context.SaveChangesAsync();
            return true;
        }

        //lấy những vị trí đc quản lý theo user
        public async Task<object> GetOrgUnitIdAttachedByUserCode(string userCode)
        {
            var results = await _context.UserMngOrgUnits
                .Where(e => e.UserCode == userCode && e.ManagementType == "MNG_TIME_KEEPING")
                .Select(e => e.OrgUnitId)
                .ToListAsync();

            return results;
        }

        //thay đổi người quản lý chấm công, thay thế người cũ sang người mới
        public async Task<object> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request)
        {
            var old = await _context.UserMngOrgUnits.Where(e => e.UserCode == request.OldUserCode).ToListAsync();

            foreach (var item in old)
            {
                item.UserCode = request.NewUserCode;
            }

            _context.UserMngOrgUnits.UpdateRange(old);

            await _context.SaveChangesAsync();

            return true;
        }

        
        //lấy id của org unit có type = tổ theo usercode
        public async Task<object> GetIdOrgUnitByUserCodeAndUnitId(string userCode, int unitId)
        {
            var data = await GetOrgUnitIdAttachedByUserCode(userCode) as List<int?>;

            var results = await _context.OrgUnits.Where(e => data != null && data.Contains(e.Id) && e.UnitId == unitId).ToListAsync();

            return results;
        }

        public async Task<object> GetDeptUserMngTimeKeeping(string userCode)
        {
            var data = await GetOrgUnitIdAttachedByUserCode(userCode) as List<int?>;

            var results = await _context.OrgUnits.Where(e => data != null && data.Contains(e.Id) && e.UnitId == (int)UnitEnum.Phong).ToListAsync();

            return results;
        }

        /// <summary>
        /// Cập nhật chấm công, chỉ đc cập nhật khi chưa được gửi tới HR
        /// </summary>
        public async Task<object> EditTimeKeeping(CreateTimeAttendanceRequest request)
        {
            var itemAttendance = await _context.TimeAttendanceEditHistories.FirstOrDefaultAsync(e => e.Datetime == request.Datetime && e.UserCode == request.UserCode);

            if (itemAttendance != null)
            {
                itemAttendance.OldValue = request.OldValue;
                itemAttendance.CurrentValue = request.CurrentValue;
                itemAttendance.UserCodeUpdate = request.UserCodeUpdate;
                itemAttendance.UpdatedBy = request.UpdatedBy;
                itemAttendance.UpdatedAt = DateTimeOffset.Now;
                itemAttendance.IsSentToHR = false;

                _context.TimeAttendanceEditHistories.Update(itemAttendance);
            }
            else
            {
                var data = new TimeAttendanceEditHistory
                {
                    Datetime = request.Datetime,
                    UserCode = request.UserCode,
                    OldValue = request.OldValue,
                    CurrentValue = request.CurrentValue,
                    UserCodeUpdate = request.UserCodeUpdate,
                    UpdatedBy = request.UpdatedBy,
                    UpdatedAt = DateTimeOffset.Now,
                    IsSentToHR = false
                };

                _context.TimeAttendanceEditHistories.Add(data);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Lấy danh sách lịch sử đã cập nhật chấm công
        /// </summary>
        public async Task<PagedResults<TimeAttendanceHistoryDto>> GetListHistoryEditTimeKeeping(GetListHistoryEditTimeKeepingRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                SELECT 
                    TA.Id, TA.Datetime, TA.UserCode, TA.OldValue, TA.CurrentValue, TA.UserCodeUpdate, TA.UpdatedBy, TA.UpdatedAt, TA.IsSentToHR,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbWeb}.dbo.time_attendance_edit_histories AS TA
                INNER JOIN 
                    {Global.DbViClock}.dbo.tblNhanVien AS NV 
                ON TA.UserCode = NV.NVMaNV AND UserCodeUpdate = @UserCodeUpdated
                ORDER BY TA.UpdatedAt DESC
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
            ";

            var totalItems = await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdate == request.UserCodeUpdated).CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var result = (await connection.QueryAsync<TimeAttendanceHistoryDto>(sql, new
            {
                Page = (int)page,
                PageSize = (int)pageSize,
                request.UserCodeUpdated
            })).ToList();

            return new PagedResults<TimeAttendanceHistoryDto>
            {
                Data = result,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Xóa lịch sử bản ghi chưa gửi đến hr
        /// </summary>
        public async Task<object> DeleteHistoryEditTimeKeeping(int id)
        {
            var item = await _context.TimeAttendanceEditHistories.FirstOrDefaultAsync(e => e.Id == id && e.IsSentToHR == false) ?? throw new NotFoundException("History time attendance not found");

            _context.TimeAttendanceEditHistories.Remove(item);

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Đếm những bản ghi của người hiện tại cập nhật chấm công và có trạng thái chưa confirm HR
        /// </summary>
        public async Task<int> CountHistoryEditTimeKeepingNotSendHR(string userCode)
        {
            return await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdate == userCode && e.IsSentToHR == false).CountAsync();
        }
    }
}
