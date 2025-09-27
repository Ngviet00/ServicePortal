using ClosedXML.Excel;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Domain.Entities;
using System.Globalization;

namespace ServicePortals.Infrastructure.Excel
{
    public class ExcelService
    {
        private readonly ApplicationDbContext _context;

        public ExcelService (ApplicationDbContext context)
        {
            _context = context;
        }

        public byte[] ExportOverTimeToExcel(ApplicationForm applicationForm)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("BANG DANG KY CA");

            ws.RowHeight = 23;

            // Header title
            ws.Cell("A1").Value = "Danh sách tăng ca";
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Merge tiêu đề
            ws.Range("A1:H1").Merge().Style.Font.SetBold().Font.FontSize = 14;

            ws.Columns("A:B").Width = 13;
            ws.Columns("C:H").Width = 18;
            ws.Column("B").Width = 20;
            ws.Column("C").Width = 22;
            ws.Column("G").Width = 30;

            ws.Cell("A2").Value = "Đơn vị";
            ws.Cell("B2").Value = applicationForm?.OrgUnitCompany?.Name;

            ws.Cell("A3").Value = "Loại tăng ca";
            ws.Cell("B3").Value = applicationForm?.TypeOverTime?.Name;

            ws.Cell("A4").Value = "Ngày Đký";
            ws.Cell("B4").Value = applicationForm?.DateRegister?.ToString("yyyy-MM-dd");

            ws.Cell("A5").Value = "Bộ phận";
            ws.Cell("B5").Value = applicationForm?.OrgUnit?.Name;


            // Cột tiêu đề bảng
            var headers = new[] { "Mã NV", "Họ tên", "Chức vụ", "Từ giờ (h)", "Đến giờ (h)", "Số giờ (h)", "Ghi chú"};
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(7, i + 1);
                cell.Value = headers[i];

                cell.Style.Font.SetBold();

                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            var overTimes = applicationForm!.ApplicationFormItems
                .SelectMany(x => x.OverTimes)
                .ToList();

            // Dữ liệu
            for (int i = 0; i < overTimes.Count; i++)
            {
                var item = overTimes[i];
                int row = i + 8;

                ws.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                ws.Cell(row, 1).Value = item?.UserCode ?? "";       
                ws.Cell(row, 2).Value = item?.UserName ?? "";  
                ws.Cell(row, 3).Value = item?.Position ?? "";
                ws.Cell(row, 4).Value = item?.FromHour ?? "";
                ws.Cell(row, 5).Value = item?.ToHour ?? "";
                ws.Cell(row, 6).Value = item?.NumberHour;
                ws.Cell(row, 7).Value = item?.Note ?? "";
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] ExportLeaveRequestToExcel(List<LeaveRequest> leaveRequests)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("BANG DANG KY CA");

            ws.RowHeight = 23;

            // Header title
            ws.Cell("A1").Value = "Danh sách nghỉ phép";
            ws.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell("A1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // Merge tiêu đề
            ws.Range("A1:H1").Merge().Style.Font.SetBold().Font.FontSize = 14;

            ws.Columns("A:B").Width = 13;
            ws.Columns("C:H").Width = 18;

            // Cột tiêu đề bảng
            var headers = new[] { "Mã NV", "Lý do nghỉ", "Ngày bắt đầu", "Ngày kết thúc", "Kiểu Nghỉ", "Ghi Chú", "Từ ngày", "Đến ngày" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(3, i + 1);
                cell.Value = headers[i];

                cell.Style.Font.SetBold();
                
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                cell.Style.Fill.BackgroundColor = (headers[i] == "Từ ngày") ? XLColor.Yellow : headers[i] == "Đến ngày" ? XLColor.LightBlue : XLColor.NoColor;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }

            // Dữ liệu
            for (int i = 0; i < leaveRequests.Count; i++)
            {
                var item = leaveRequests[i];
                int row = i + 4;

                ws.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Row(row).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                ws.Cell(row, 1).Value = item?.UserCode;
                ws.Cell(row, 2).Value = item?.TypeLeave?.Code;

                var dateFromStr = item!.FromDate?.ToString("yyyy-MM-dd HH:mm") ?? DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm");
                var dateToStr = item!.ToDate?.ToString("yyyy-MM-dd HH:mm") ?? DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm");

                ws.Cell(row, 3).Value = dateFromStr;
                ws.Cell(row, 4).Value = dateToStr;

                ws.Cell(row, 5).Value = item?.TimeLeave?.Name;
                ws.Cell(row, 6).Value = "";
                ws.Cell(row, 7).Value = item!.FromDate != null ? ((DateTimeOffset)item.FromDate).DateTime : DateTimeOffset.Now.ToString("d-MMM", CultureInfo.InvariantCulture);
                ws.Cell(row, 8).Value = item!.ToDate != null ? ((DateTimeOffset)item.ToDate).DateTime : DateTimeOffset.Now.ToString("d-MMM", CultureInfo.InvariantCulture);
            }

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        public byte[] GenerateExcelHistoryEditTimeKeeping(List<TimeAttendanceEditHistory> data)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DanhSachChinhSuaChamCong");

                worksheet.Cell(1, 1).Value = "Mã NV (Được Sửa)";
                worksheet.Cell(1, 2).Value = "Ngày Chấm Công";
                worksheet.Cell(1, 3).Value = "Giá Trị Cũ";
                worksheet.Cell(1, 4).Value = "Giá Trị Mới";

                worksheet.Row(1).Style.Font.SetBold();
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.UserCode;
                    worksheet.Cell(row, 2).Value = item.Datetime != null ? item.Datetime.Value.ToString("yyyy-MM-dd") : "";
                    worksheet.Cell(row, 3).Value = item.OldValue;
                    worksheet.Cell(row, 4).Value = item.CurrentValue;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public void GenerateExcelManagerConfirmToHR(XLWorkbook workbook, List<dynamic> rows, bool isFirstBatch, int daysInMonth, int month, int year)
        {
            var worksheet = workbook.Worksheets.FirstOrDefault() ?? workbook.AddWorksheet("CHẤM CÔNG");
            worksheet.TabColor = XLColor.Green;
            int startColumn = 3;

            int startRow = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 1;

            if (isFirstBatch && startRow == 1)
            {
                worksheet.Cell(startRow, 1).Value = "Mã Nhân Viên";
                worksheet.Cell(startRow, 2).Value = "Họ Tên";
                worksheet.Cell(startRow, 3).Value = "Bộ Phận";

                worksheet.Column(1).Width = 12;
                worksheet.Column(2).Width = 18;
                worksheet.Column(3).Width = 13;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    worksheet.Cell(startRow, startColumn + day).Value = $"{day:D2}";
                }

                worksheet.Range(startRow, 1, startRow, daysInMonth + startColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(startRow, 1, startRow, daysInMonth + startColumn).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                worksheet.Columns(5, daysInMonth + startColumn + 1).Width = 6;

                startRow++;
            }

            var headerRow = worksheet.Row(1);
            headerRow.Height = 23;
            headerRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            headerRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int batchDataStartRow = startRow;

            foreach (var row in rows)
            {
                var rowAsDict = (IDictionary<string, object>)row;

                int col = 1;
                worksheet.Cell(startRow, col++).Value = row.UserCode;
                worksheet.Cell(startRow, col++).Value = row.Name;
                worksheet.Cell(startRow, col++).Value = row.Department;


                for (int d = 1; d <= daysInMonth; d++)
                {
                    string bgColor = "#FFFFFF";
                    string textColor = "#000000";
                    bool isSunday = false;

                    string value = rowAsDict[$"ATT{d}"]?.ToString() ?? "";
                    string den = rowAsDict[$"Den{d}"]?.ToString() ?? "";
                    string ve = rowAsDict[$"Ve{d}"]?.ToString() ?? "";
                    string wh = rowAsDict[$"WH{d}"]?.ToString() ?? "";
                    string ot = rowAsDict[$"OT{d}"]?.ToString() ?? "";

                    var currentCell = worksheet.Cell(startRow, col);

                    isSunday = DateTime.TryParseExact($"{year}-{month:D2}-{d:D2}", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var dt) && dt.DayOfWeek == DayOfWeek.Sunday;

                    if (value == "X")
                    {
                        if (isSunday)
                        {
                            value = "CN_X";
                        }
                        else if (float.Parse(wh) == 7 || float.Parse(wh) == 8)
                        {
                            value = "X";
                        }
                        else if (float.Parse(wh) < 8)
                        {
                            var calculateTime = Helper.CalculateRoundedTime(8 - float.Parse(wh));
                            value = calculateTime == "1" ? "X" : calculateTime;
                        }
                    }
                    else if (value == "SH")
                    {
                        bgColor = "#3AFD13";
                    }
                    else if (value == "CN")
                    {
                        if (den != "" && ve != "" && (float.Parse(wh) != 0 || float.Parse(ot) != 0))
                        {
                            value = "CN_X";
                            bgColor = "#FFFFFF";
                            textColor = "#000000";
                        }
                        else
                        {
                            bgColor = "#858585";
                        }
                    }
                    else if (value == "ABS" || value == "MISS")
                    {
                        bgColor = "#FD5C5E";
                    }
                    else if (value != "X")
                    {
                        bgColor = "#ffe378";
                    }

                    currentCell.Value = value;
                    currentCell.Style.Font.FontColor = XLColor.FromHtml(textColor);
                    currentCell.Style.Fill.SetBackgroundColor(XLColor.FromHtml(bgColor));

                    col++;
                }

                var currentRow = worksheet.Row(startRow);
                currentRow.Height = 20;
                currentRow.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                currentRow.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Range(startRow, 1, startRow, daysInMonth + startColumn).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                worksheet.Range(startRow, 1, startRow, daysInMonth + startColumn).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);

                startRow++;
            }
        }
    }
}
