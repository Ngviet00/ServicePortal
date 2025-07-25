using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using ServicePortals.Infrastructure.Data;
using System.Text;
using Dapper;

namespace ServicePortals.Infrastructure.Excel
{
    public class ExcelService
    {
        private readonly ApplicationDbContext _context;

        public ExcelService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InsertFromExcelAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File không hợp lệ");
            }

            var insertBuilder = new StringBuilder();
            insertBuilder.Append("INSERT INTO YourTable (Name, Age) VALUES ");

            var valuesList = new List<string>();
            var parameters = new DynamicParameters();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed();

                    int index = 0;
                    foreach (var row in rows.Skip(1))
                    {
                        var name = row.Cell(1).GetString();
                        var ageCell = row.Cell(2);
                        int age = 0;

                        if (ageCell.DataType == XLDataType.Number)
                            age = (int)ageCell.GetDouble();
                        else if (int.TryParse(ageCell.GetString(), out int parsedAge))
                            age = parsedAge;

                        // Tạo tham số cho từng bản ghi
                        string paramName = "@Name" + index;
                        string paramAge = "@Age" + index;

                        valuesList.Add($"({paramName}, {paramAge})");
                        parameters.Add(paramName, name);
                        parameters.Add(paramAge, age);

                        index++;
                    }
                }
            }

            if (valuesList.Count > 0)
            {
                var sql = insertBuilder.ToString() + string.Join(", ", valuesList);
                //await _connection.ExecuteAsync(sql, parameters);
            }
        }

        public byte[] ExportLeaveRequestToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Data");

                // Thêm tiêu đề
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Date";

                //// Thêm dữ liệu
                //for (int i = 0; i < data.Count; i++)
                //{
                //    var row = i + 2;
                //    worksheet.Cell(row, 1).Value = data[i].Id;
                //    worksheet.Cell(row, 2).Value = data[i].Name;
                //    worksheet.Cell(row, 3).Value = data[i].Date;
                //}

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] GenerateExcelManagerConfirmToHR()
        {
            //var holidays = data.Holidays;
            //var userData = data.UserData;

            //if (userData == null || userData.Count == 0 || userData[0].Attendances == null)
            //    return [];

            //int daysInMonth = userData[0].Attendances.Count;

            //using var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Attendance");

            ////row start list attendance
            //int startRow = 8;

            ////display month, year
            //var cellMonthYear = worksheet.Cell(3, 16);
            //cellMonthYear.Value = $"{request.Month} - {request.Year}";
            //cellMonthYear.Style.Font.Bold = true;
            //cellMonthYear.Style.Font.FontSize = 28;
            //cellMonthYear.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //cellMonthYear.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //worksheet.Row(3).Height = 24;

            ////display color
            //int startColumnColor = 3;
            //int startRowColor = 5;

            //if (request.StatusColors != null && request.StatusDefine != null)
            //{
            //    foreach (var (status, colorHex) in request.StatusColors)
            //    {
            //        var statusCell = worksheet.Cell(startRowColor, startColumnColor);
            //        statusCell.Value = status;
            //        statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml(colorHex);
            //        statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //        var cellRange = worksheet.Range(startRowColor, startColumnColor + 1, startRowColor, startColumnColor + 2);
            //        cellRange.Merge();
            //        if (request.StatusDefine.TryGetValue(status, out var description))
            //        {
            //            cellRange.Value = description;
            //        }
            //        else
            //        {
            //            cellRange.Value = "";
            //        }

            //        cellRange.Style.Font.FontSize = 10;
            //        cellRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

            //        startColumnColor += 4;
            //    }
            //}


            // === Header ===
            //var headerRange = worksheet.Range(startRow, 1, startRow, daysInMonth + 2);
            //headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#d1d5dc");
            //headerRange.Style.Font.FontColor = XLColor.Black;
            //headerRange.Style.Font.FontSize = 12;
            //headerRange.Style.Font.Bold = true;
            //headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            //worksheet.Row(startRow).Height = 24;

            //for (int day = 1; day <= daysInMonth + 2; day++)
            //{
            //    int colIndex = day;

            //    var cell = worksheet.Cell(startRow, colIndex);
            //    cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            //    cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            //    cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            //    cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;

            //    if (colIndex == 1)
            //    {
            //        worksheet.Column(colIndex).Width = 22;
            //        cell.Value = "Mã Nhân Viên";
            //    }
            //    else if (colIndex == 2)
            //    {
            //        worksheet.Column(colIndex).Width = 25;
            //        cell.Value = "Họ Tên";
            //    }
            //    else
            //    {
            //        worksheet.Column(colIndex).Width = 6;
            //        cell.Value = (colIndex - 2).ToString("D2");

            //        //var dayStr = $"{request?.Year}-{request?.Month?.ToString("D2")}-{(colIndex - 2).ToString("D2")}";

            //        //var holiday = holidays?.FirstOrDefault(h => h.Date == dayStr);

            //        //if (holiday != null)
            //        //{
            //        //    if (holiday.Type == "sunday")
            //        //    {
            //        //        cell.Style.Fill.BackgroundColor = XLColor.Black;
            //        //        cell.Style.Font.FontColor = XLColor.White;
            //        //    }
            //        //    else if (holiday.Type == "special_holiday")
            //        //    {
            //        //        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#07ee15");
            //        //        cell.Style.Font.FontColor = XLColor.Black;
            //        //    }
            //        //}
            //    }
            //}

            // === Body ===
            //for (int row = 0; row < userData.Count; row++)
            //{
            //    var item = userData[row];
            //    var lengthAttendaces = item.Attendances != null ? item.Attendances.Count : 0;
            //    worksheet.Row(row + 2 + (startRow - 1)).Height = 24;

            //    if (item.Attendances != null)
            //    {
            //        for (int col = 1; col <= lengthAttendaces + 2; col++)
            //        {
            //            var cell = worksheet.Cell(row + 2 + (startRow - 1), col);
            //            cell.Style.Border.TopBorder = XLBorderStyleValues.Thin;
            //            cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            //            cell.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            //            cell.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            //            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            //            if (col == 1)
            //            {
            //                worksheet.Column(col).Width = 22;
            //                cell.Value = item.UserCode;
            //            }
            //            else if (col == 2)
            //            {
            //                worksheet.Column(col).Width = 25;
            //                cell.Value = $"Name_{item.UserCode}";
            //            }
            //            else
            //            {
            //                int attIndex = col - 3;
            //                if (attIndex >= 0 && attIndex < item.Attendances.Count)
            //                {
            //                    var att = item.Attendances[attIndex];
            //                    worksheet.Column(col).Width = 6;
            //                    cell.Value = att?.Status ?? "";
            //                    if (!string.IsNullOrWhiteSpace(att?.Status) && request.StatusColors != null && request.StatusColors.TryGetValue(att.Status, out var hexColor))
            //                    {
            //                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml(hexColor);
            //                    }
            //                    else
            //                    {
            //                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#f7f7f7");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            using var stream = new MemoryStream();
            //workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
