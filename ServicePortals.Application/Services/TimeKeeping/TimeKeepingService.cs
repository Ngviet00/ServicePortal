using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using Dapper;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.TimeKeeping;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.TimeKeeping
{
    public class TimeKeepingService : ITimeKeepingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly IEmailService _emailService;

        public TimeKeepingService (
            IViclockDapperContext viclockDapperContext, 
            IEmailService emailService, 
            ApplicationDbContext context
        )
        {
            _viclockDapperContext = viclockDapperContext;
            _emailService = emailService;
            _context = context;
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
            int month = request.Month ?? DateTime.UtcNow.Month;
            int year = request.Year ?? DateTime.UtcNow.Year;


            int dayInMonth = DateTime.DaysInMonth(year, month);

            double pageSize = request.PageSize > 0 ? request.PageSize : 10;
            double page = request.Page > 0 ? request.Page : 1;
            string keySearch = request.keySearch?.Trim() ?? "";

            int? deptId = request.DeptId;
            int? team = request.Team;

            var fromDate = $"{year}-{month:D2}-01";
            var toDate = $"{year}-{month:D2}-{dayInMonth}";

            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

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

        public void WriteToExcel(List<AttendanceExportRow> rows, string filePath, bool isFirstBatch)
        {
            using var workbook = isFirstBatch && System.IO.File.Exists(filePath)
                ? new XLWorkbook(filePath)
                : new XLWorkbook();

            var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.AddWorksheet("Attendance");

            int startRow = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;

            if (isFirstBatch && startRow == 1)
            {
                worksheet.Cell(startRow, 1).Value = "UserCode";
                worksheet.Cell(startRow, 2).Value = "FullName";
                worksheet.Cell(startRow, 3).Value = "JoinDate";

                for (int day = 1; day <= 31; day++)
                    worksheet.Cell(startRow, 3 + day).Value = $"Day {day}";

                startRow++;
            }

            foreach (var row in rows)
            {
                int col = 1;
                worksheet.Cell(startRow, col++).Value = row.UserCode;
                worksheet.Cell(startRow, col++).Value = row.FullName;
                worksheet.Cell(startRow, col++).Value = row.JoinDate == DateTime.MinValue ? "" : row.JoinDate.ToString("yyyy-MM-dd");

                for (int d = 1; d <= 31; d++)
                {
                    worksheet.Cell(startRow, col++).Value = row.DayValues.GetValueOrDefault(d, "");
                }

                startRow++;
            }

            workbook.SaveAs(filePath);
        }

        public byte[] BuildExcelFile(List<AttendanceExportRow> rows)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Attendance");

            int startRow = 1;

            // Header
            worksheet.Cell(startRow, 1).Value = "UserCode";
            worksheet.Cell(startRow, 2).Value = "FullName";
            worksheet.Cell(startRow, 3).Value = "JoinDate";

            for (int day = 1; day <= 31; day++)
                worksheet.Cell(startRow, 3 + day).Value = $"Day {day}";

            startRow++;

            // Body
            foreach (var row in rows)
            {
                int col = 1;
                worksheet.Cell(startRow, col++).Value = row.UserCode;
                worksheet.Cell(startRow, col++).Value = row.FullName;
                worksheet.Cell(startRow, col++).Value = row.JoinDate == DateTime.MinValue ? "" : row.JoinDate.ToString("yyyy-MM-dd");

                for (int d = 1; d <= 31; d++)
                {
                    worksheet.Cell(startRow, col++).Value = row.DayValues.GetValueOrDefault(d, "");
                }

                startRow++;
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray(); // trả về nội dung file Excel
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

            const string filePath = @"E:\ChamCong_T6_2025.xlsx";

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sqlCombinedQuery = $@"
                WITH RecursiveOrg AS (
                    -- CTE đệ quy để lấy tất cả OrgUnitId con
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
                    -- Lấy danh sách các OrgUnitId duy nhất
                    SELECT DISTINCT id FROM RecursiveOrg
                )
                SELECT
                    NV.NVMa,
                    NV.NVMaNV,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen,
                    NV.OrgUnitID,
                    BP.BPTen
                FROM {Global.DbViClock}.[dbo].[tblNhanVien] AS NV
                LEFT JOIN {Global.DbViClock}.[dbo].[tblBoPhan] AS BP ON BP.BPMa = NV.NVMaBP
                WHERE
                    NV.OrgUnitID IN (SELECT id FROM DistinctOrgUnits) -- Sử dụng kết quả từ CTE trên
                    AND NV.NVNgayRa > GETDATE();
            ";

            var paramCombined = new
            {
                userCode = request.UserCode,
                type = "MNG_TIME_KEEPING",
            };

            var allUsers = await connection.QueryAsync<GetMultiUserViClockByOrgUnitIdConfirmTimeKeepingResponse>(sqlCombinedQuery, paramCombined);

            int totalUsers = allUsers.Count();
            int totalBatches = (int)Math.Ceiling((double)totalUsers / batchSize);

            var allExportRows = new List<AttendanceExportRow>();

            for (int i = 0; i < totalBatches; i++)
            {
                var currentBatch = allUsers.Skip(i * batchSize).Take(batchSize).ToList();

                var userIds = currentBatch.Select(u => u.NVMa).Distinct().ToList();

                var sqlGetTimeekping = StringSqlGetTimeekping();

                var attendanceLists = (await connection.QueryAsync<GetUserTimeKeepingResponse>(sqlGetTimeekping, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    UserIds = userIds
                })).ToList();

                //var exportRows = BuildExportRows(currentBatch, attendanceLists, month, year);

                //WriteToExcel(exportRows, filePath, i == 0);
                var exportRows = BuildExportRows(currentBatch, attendanceLists, month, year);
                allExportRows.AddRange(exportRows);
            }

            var excelBytes = BuildExcelFile(allExportRows);

            var attachments = new List<(string FileName, byte[] FileBytes)>
            {
                ("ChamCong_2025-08-02.xlsx", excelBytes)
            };

            await _emailService.EmailSendTimeKeepingToHR(new List<string> { "nguyenviet@vsvn.com.vn" },
                    new List<string> { Global.EmailDefault },
                    "test",
                    "avbc",
                    attachments,
                    false);

            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailAsync(
            //        new List<string> { Global.EmailDefault },
            //        new List<string> { Global.EmailDefault },
            //        "test",
            //        "avbc",
            //        attachments,
            //        false
            //    )
            //);


            //const string filePath = @"E:\ChamCong_T6_2025.xlsx";

            //var allUserCodes = Enumerable.Range(1, TotalUsers)
            //                         .Select(i => $"U{i:0000}")
            //                         .ToList();

            //if (System.IO.File.Exists(filePath))
            //    System.IO.File.Delete(filePath);


            //using var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Attendance");

            //// Header
            //worksheet.Cell(1, 1).Value = "UserCode";
            //for (int day = 1; day <= daysInMonth; day++)
            //{
            //    worksheet.Cell(1, day + 1).Value = $"Day {day}";
            //}

            //int currentRow = 2;

            //for (int i = 0; i < allUserCodes.Count; i += batchSize)
            //{
            //    var batch = allUserCodes.Skip(i).Take(batchSize).ToList();
            //    var attendanceBatch = GenerateAttendance(batch);

            //    foreach (var userCode in batch)
            //    {
            //        var records = attendanceBatch.Where(x => x.UserCode == userCode).ToList();

            //        worksheet.Cell(currentRow, 1).Value = userCode;
            //        for (int day = 1; day <= daysInMonth; day++)
            //        {
            //            var dayRecord = records.FirstOrDefault(r => r.Day == day);
            //            worksheet.Cell(currentRow, day + 1).Value = dayRecord?.Status ?? "";
            //        }

            //        currentRow++;
            //    }

            //    Console.WriteLine($"Đã xử lý xong batch từ {i} đến {i + batch.Count - 1}");
            //}

            //// Style nhẹ
            //worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            //worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Hair;

            //workbook.SaveAs(filePath);
            //Console.WriteLine($"Đã lưu file Excel vào {Path.GetFullPath(filePath)}");

            //int totalEmployees = 2000;
            //int batchSize = 500;
            //var startDate = new DateTime(2025, 6, 1);
            //var endDate = new DateTime(2025, 6, 30);

            //var wb = System.IO.File.Exists(filePath) ? new XLWorkbook(filePath) : new XLWorkbook();
            //var ws = wb.Worksheets.Contains("ChamCong") ? wb.Worksheet("ChamCong") : wb.AddWorksheet("ChamCong");

            //int currentRow = 1;

            //// Ghi header nếu dòng đầu chưa có
            //if (ws.Cell(1, 1).GetString() != "STT")
            //{
            //    ws.Cell(currentRow, 1).Value = "STT";
            //    ws.Cell(currentRow, 2).Value = "MaNV";
            //    ws.Cell(currentRow, 3).Value = "HoTen";
            //    ws.Cell(currentRow, 4).Value = "Ngay";
            //    ws.Cell(currentRow, 5).Value = "GioVao";
            //    ws.Cell(currentRow, 6).Value = "GioRa";
            //    currentRow++;
            //}
            //else
            //{
            //    currentRow = ws.LastRowUsed().RowNumber() + 1;
            //}

            //for (int i = 0; i < totalEmployees; i += batchSize)
            //{
            //    var batch = GenerateChamCongBatch(i + 1, Math.Min(i + batchSize, totalEmployees), startDate, endDate);

            //    foreach (var item in batch)
            //    {
            //        ws.Cell(currentRow, 1).Value = item.STT;
            //        ws.Cell(currentRow, 2).Value = item.MaNV;
            //        ws.Cell(currentRow, 3).Value = item.HoTen;
            //        ws.Cell(currentRow, 4).Value = item.Ngay;
            //        ws.Cell(currentRow, 5).Value = item.GioVao;
            //        ws.Cell(currentRow, 6).Value = item.GioRa;
            //        currentRow++;
            //    }

            //    Console.WriteLine($"Batch từ {i + 1} đến {Math.Min(i + batchSize, totalEmployees)} đã xong.");
            //}

            //wb.SaveAs(filePath);
            //Console.WriteLine("Tạo file xong tại: " + filePath);


            //var data = GetFakeData(); // hoặc gọi từ DbContext

            //var fileName = $"Report_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            //var filePath = Path.Combine(Path.GetTempPath(), fileName);

            //// Tạo Excel bằng ClosedXML
            //using (var workbook = new XLWorkbook())
            //{
            //    var ws = workbook.Worksheets.Add("Báo Cáo");
            //    ws.Cell(1, 1).InsertTable(data);
            //    workbook.SaveAs(filePath);
            //}

            //// Gửi mail kèm file
            //await _emailService.SendEmailAsync(
            //    "hr@congty.com",
            //    "Báo cáo dữ liệu ngày " + DateTime.Now.ToString("dd/MM/yyyy"),
            //    "File báo cáo đính kèm.",
            //    new[] { filePath });

            //File.Delete(filePath); // dọn file

            //using var stream = new MemoryStream();
            //var fileBytes = stream.ToArray();

            //string subject = $"Production - Confirm Attendance List [{request.Month} - {request.Year}]";
            //string bodyMail = $@"Dear HR Team, Please find attached the excel file containing the staff attendance list [{request.Month} - {request.Year}]";
            //var emailHrMngTimeKeeping = await _hrManagementService.GetEmailHRByType("MANAGE_TIMEKEEPING");

            //var attachments = new List<(string, byte[])>
            //{
            //    ($"Confirm_Attendance_T{request.Month} - {request.Year}.xlsx", fileBytes)
            //};

            //_emailService.EmailSendTimeKeepingToHR();

            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailAsync(
            //        new List<string> { Global.EmailDefault },
            //        new List<string> { Global.EmailDefault },
            //        subject,
            //        bodyMail,
            //        attachments,
            //        false
            //    )
            //);

            return true;
        }

        /// <summary>
        /// Build export rows from users and attendance data
        /// </summary>
        public static List<AttendanceExportRow> BuildExportRows(
            List<GetMultiUserViClockByOrgUnitIdConfirmTimeKeepingResponse> users,
            List<GetUserTimeKeepingResponse> attendances,
            int month,
            int year)
        {
            var exportRows = new List<AttendanceExportRow>();
            int daysInMonth = DateTime.DaysInMonth(year, month);

            foreach (var user in users)
            {
                var row = new AttendanceExportRow
                {
                    UserCode = user.NVMaNV ?? "",
                    FullName = user.NVHoTen ?? "",
                    DayValues = new Dictionary<int, string>()
                };

                var userAtt = attendances
                    .Where(a => a.NVMaNV == user.NVMaNV)
                    .ToDictionary(
                        a => DateTime.Parse(a.BCNgay!).Day,
                        a => (a.Results ?? "", a.CustomResult ?? "", a.IsSentToHR == true ? "✓" : ""));

                for (int day = 1; day <= daysInMonth; day++)
                {
                    if (userAtt.TryGetValue(day, out var val))
                    {
                        var (results, custom, isSent) = val;
                        var display = string.Join(" | ", new[] { results, custom, isSent }.Where(x => !string.IsNullOrWhiteSpace(x)));
                        row.DayValues[day] = display;
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

    public class AttendanceExportRow
    {
        public string UserCode { get; set; }
        public string FullName { get; set; }
        public DateTime JoinDate { get; set; }
        public Dictionary<int, string> DayValues { get; set; } = new(); // key = day 1..31
    }
}
