using ClosedXML.Excel;
using ServicePortal.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Modules.TimeKeeping.DTO.Responses;

namespace ServicePortal.Infrastructure.Excel
{
    public class ExcelService
    {
        public byte[] GenerateExcelManagerConfirmToHR(ManagementTimeKeepingResponseDto data, GetManagementTimeKeepingDto request)
        {
            var holidays = data.Holidays;
            var userData = data.UserData;

            if (userData == null || userData.Count == 0 || userData[0].Attendances == null)
                return [];


            int daysInMonth = userData[0].Attendances.Count;

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Attendance");

            //row start list attendance
            int startRow = 8;

            //display month, year
            var cellMonthYear = worksheet.Cell(3, 16);
            cellMonthYear.Value = $"{request.Month} - {request.Year}";
            cellMonthYear.Style.Font.Bold = true;
            cellMonthYear.Style.Font.FontSize = 28;
            cellMonthYear.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cellMonthYear.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Row(3).Height = 24;


            //display color
            int startColumnColor = 3;
            int startRowColor = 5;

            if (request.StatusColors != null && request.StatusDefine != null)
            {
                foreach (var (status, colorHex) in request.StatusColors)
                {
                    var statusCell = worksheet.Cell(startRowColor, startColumnColor);
                    statusCell.Value = status;
                    statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml(colorHex);
                    statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    var cellRange = worksheet.Range(startRowColor, startColumnColor + 1, startRowColor, startColumnColor + 2);
                    cellRange.Merge();
                    if (request.StatusDefine.TryGetValue(status, out var description))
                    {
                        cellRange.Value = description;
                    }
                    else
                    {
                        cellRange.Value = "";
                    }

                    cellRange.Style.Font.FontSize = 10;
                    cellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    startColumnColor += 4;
                }
            }


            // === Header ===
            var headerRange = worksheet.Range(startRow, 1, startRow, daysInMonth + 2);
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#d1d5dc");
            headerRange.Style.Font.FontColor = XLColor.Black;
            headerRange.Style.Font.FontSize = 12;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Row(startRow).Height = 24;

            for (int day = 1; day <= daysInMonth + 2; day++)
            {
                int colIndex = day;

                var cell = worksheet.Cell(startRow, colIndex);
                cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

                if (colIndex == 1)
                {
                    worksheet.Column(colIndex).Width = 22;
                    cell.Value = "Mã Nhân Viên";
                }
                else if (colIndex == 2)
                {
                    worksheet.Column(colIndex).Width = 25;
                    cell.Value = "Họ Tên";
                }
                else
                {
                    worksheet.Column(colIndex).Width = 6;
                    cell.Value = (colIndex - 2).ToString("D2");
                }
            }

            // === Body ===
            for (int row = 0; row < userData.Count; row++)
            {
                var item = userData[row];
                var lengthAttendaces = item.Attendances != null ? item.Attendances.Count : 0;
                worksheet.Row(row + 2 + (startRow - 1)).Height = 24;

                if (item.Attendances != null)
                {
                    for (int col = 1; col <= lengthAttendaces + 2; col++)
                    {
                        var cell = worksheet.Cell(row + 2 + (startRow - 1), col);
                        cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                        if (col == 1)
                        {
                            worksheet.Column(col).Width = 22;
                            cell.Value = item.UserCode;
                        }
                        else if (col == 2)
                        {
                            worksheet.Column(col).Width = 25;
                            cell.Value = $"Name_{item.UserCode}";
                        }
                        else
                        {
                            int attIndex = col - 3;
                            if (attIndex >= 0 && attIndex < item.Attendances.Count)
                            {
                                var att = item.Attendances[attIndex];
                                worksheet.Column(col).Width = 6;
                                cell.Value = att?.Status ?? "";
                                if (!string.IsNullOrWhiteSpace(att?.Status) && request.StatusColors != null && request.StatusColors.TryGetValue(att.Status, out var hexColor))
                                {
                                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml(hexColor);
                                }
                                else
                                {
                                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f7f7f7");
                                }
                            }
                        }
                    }
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
