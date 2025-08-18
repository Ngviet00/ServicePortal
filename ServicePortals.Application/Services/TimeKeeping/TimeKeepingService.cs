using System.Data;
using System.Dynamic;
using ClosedXML.Excel;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.TimeKeeping;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.TimeKeeping
{
    public class TimeKeepingService : ITimeKeepingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ExcelService _excelService;

        public TimeKeepingService(
            ExcelService excelService,
            IEmailService emailService,
            ApplicationDbContext context,
            ILeaveRequestService leaveRequestService
        )
        {
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

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var result = await _context.Database.GetDbConnection().QueryAsync<object>($"{Global.DbViClock}.dbo.cp_get_table_timekeeping", param, commandType: CommandType.StoredProcedure);

            return result;
        }

        //màn quản lý chấm công, người quản lý chấm công quản lý chấm công của người khác
        public async Task<PagedResults<dynamic>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request)
        {
            int month = request.Month ?? DateTime.UtcNow.Month;
            int year = request.Year ?? DateTime.UtcNow.Year;
            int dayInMonth = DateTime.DaysInMonth(year, month);

            double pageSize = request.PageSize > 0 ? request.PageSize : 10;
            double page = request.Page > 0 ? request.Page : 1;
            string keySearch = request.keySearch?.Trim() ?? "";
            int? deptId = request.DeptId;

            string tableName = $"BaoCaoVS5{year}_{month:D2}";

            var parameters = new DynamicParameters();

            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@MngType", "MNG_TIME_KEEPING", DbType.String, ParameterDirection.Input);
            parameters.Add("@KeySearch", request.keySearch?.Trim() ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@DepartmentId", request.DeptId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TableName", tableName, DbType.String, ParameterDirection.Input);
            parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                    .QueryAsync<dynamic>(
                        "dbo.usp_GetMngTimeKeepingData",
                        parameters,
                        commandType: CommandType.StoredProcedure,
                        commandTimeout: 900
            );

            int totalCount = parameters.Get<int>("@TotalCount");
            int totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var finalResults = FormatDataGetUserAndTimeKeeping((List<dynamic>)results, dayInMonth);

            return new PagedResults<dynamic>
            {
                Data = finalResults,
                TotalItems = totalCount,
                TotalPages = totalPages
            };
        }

        //gửi chấm công cho bộ phận HR
        public async Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request)
        {
            int batchSize = 200;

            int month = request.Month ?? DateTime.Now.Month;
            int year = request.Year ?? DateTime.Now.Year;
            int daysInMonth = DateTime.DaysInMonth(year, month);

            var parameters = new DynamicParameters();
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@MngType", "MNG_TIME_KEEPING", DbType.String, ParameterDirection.Input);
            parameters.Add("@KeySearch", null, DbType.String, ParameterDirection.Input);
            parameters.Add("@DepartmentId", null, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Page", 1, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", 100000, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TableName", $"BaoCaoVS5{year}_{month:D2}", DbType.String, ParameterDirection.Input);
            parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            
            var listEditTimeKeeping = await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdated == request.UserCode && e.IsSentToHR == false).ToListAsync();
            //update edit timekeeping to status send to HR
            await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdated == request.UserCode && e.IsSentToHR == false).ExecuteUpdateAsync(s => s.SetProperty(e => e.IsSentToHR, true));
            //export excel list edit timekeeping
            byte[] excelFileEditHistoryTimeKeeping = _excelService.GenerateExcelHistoryEditTimeKeeping(listEditTimeKeeping);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<dynamic>(
                    "dbo.usp_GetMngTimeKeepingData",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: 900
            );

            var finalResults = FormatDataGetUserAndTimeKeeping((List<dynamic>)results, daysInMonth);

            int total = finalResults.Count;

            int totalBatches = (int)Math.Ceiling((double)total / batchSize);

            using var workbook = new XLWorkbook();

            for (int i = 0; i < totalBatches; i++)
            {
                var currentBatch = finalResults.Skip(i * batchSize).Take(batchSize).ToList();
                _excelService.GenerateExcelManagerConfirmToHR(workbook, currentBatch, i == 0, daysInMonth, month, year);
            }

            byte[] excelBytes;
            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                excelBytes = stream.ToArray();
            }

            List<(string FileName, byte[] FileBytes)> attachments =
            [
                ($"BangChamCong_{year}-{request.Month:D2}.xlsx", excelBytes),
                ($"DanhSachChinhSua.xlsx", excelFileEditHistoryTimeKeeping)
            ];

            //string outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TestOutput");
            //Directory.CreateDirectory(outputDirectory);

            //foreach (var attachment in attachments)
            //{
            //    string filePath = Path.Combine(outputDirectory, attachment.FileName);
            //    await System.IO.File.WriteAllBytesAsync(filePath, attachment.FileBytes);
            //}

            //return true;

            var hrHavePermissionMngLeaveRequest = await _leaveRequestService.GetHrWithManagementLeavePermission();
            var emailUserSend = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode);
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

        public static List<dynamic> FormatDataGetUserAndTimeKeeping(List<dynamic> rawData, int dayInMonth)
        {
            var finalResults = new List<dynamic>();

            var userGroups = rawData.GroupBy(r => r.NVMaNV);

            foreach (var group in userGroups)
            {
                var originalDataRow = group.FirstOrDefault(r => r.Datetime == null);

                if (originalDataRow == null)
                {
                    originalDataRow = group.FirstOrDefault();
                    if (originalDataRow == null)
                    {
                        continue;
                    }
                }

                dynamic displayRow = new ExpandoObject();
                var displayRowDict = (IDictionary<string, object>)displayRow;

                displayRowDict["UserCode"] = originalDataRow.NVMaNV ?? "";
                displayRowDict["Name"] = originalDataRow.Format_NVHoTen ?? "";
                displayRowDict["Department"] = originalDataRow.DepartmentName ?? "";

                var customValuesByDay = group.Where(r => r.Datetime != null).ToDictionary(r => r.Datetime.Day, r => r.CurrentValue);

                var fieldPrefixes = new[] { "ATT", "Den", "Ve", "WH", "OT" };
                for (int day = 1; day <= dayInMonth; day++)
                {
                    foreach (var prefix in fieldPrefixes)
                    {
                        string fieldName = $"{prefix}{day}";
                        string finalValue = "";

                        var originalDataDict = (IDictionary<string, object>)originalDataRow;

                        if (prefix == "ATT" && customValuesByDay.TryGetValue(day, out var customValue))
                        {
                            finalValue = customValue;
                        }
                        else
                        {
                            originalDataDict.TryGetValue(fieldName, out var originalValue);
                            finalValue = originalValue?.ToString() ?? "";
                        }

                        displayRowDict[fieldName] = finalValue;
                    }
                }
                finalResults.Add(displayRow);
            }

            return finalResults;
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
            var userMngOrgUnitIds = await _context.UserMngOrgUnitId.Where(e => e.UserCode == request.UserCode && e.ManagementType == "MNG_TIME_KEEPING").ToListAsync();

            var existingIds = userMngOrgUnitIds.Select(e => e.OrgUnitId).ToHashSet();
            var newIds = request.OrgUnitIds.ToHashSet();

            _context.UserMngOrgUnitId.RemoveRange(userMngOrgUnitIds.Where(e => !newIds.Contains((int)(e?.OrgUnitId))));

            _context.UserMngOrgUnitId.AddRange(request.OrgUnitIds
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
        public async Task<object> GetOrgUnitIdMngByUser(string userCode)
        {
            var results = await _context.UserMngOrgUnitId
                .Where(e => e.UserCode == userCode && e.ManagementType == "MNG_TIME_KEEPING")
                .Select(e => e.OrgUnitId)
                .ToListAsync();

            return results;
        }

        //thay đổi người quản lý chấm công, thay thế người cũ sang người mới
        public async Task<object> ChangeUserMngTimeKeeping(ChangeUserMngTimeKeepingRequest request)
        {
            var old = await _context.UserMngOrgUnitId.Where(e => e.UserCode == request.OldUserCode).ToListAsync();

            foreach (var item in old)
            {
                item.UserCode = request.NewUserCode;
            }

            _context.UserMngOrgUnitId.UpdateRange(old);

            await _context.SaveChangesAsync();

            return true;
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
                itemAttendance.UserCodeUpdated = request.UserCodeUpdate;
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
                    UserCodeUpdated = request.UserCodeUpdate,
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
                    TA.Id, TA.Datetime, TA.UserCode, TA.OldValue, TA.CurrentValue, TA.UserCodeUpdated, TA.UpdatedBy, TA.UpdatedAt, TA.IsSentToHR,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbWeb}.dbo.time_attendance_edit_histories AS TA
                INNER JOIN 
                    {Global.DbViClock}.dbo.tblNhanVien AS NV 
                ON TA.UserCode = NV.NVMaNV AND UserCodeUpdated = @UserCodeUpdated
                ORDER BY TA.UpdatedAt DESC
                OFFSET (@Page - 1) * @PageSize ROWS
                FETCH NEXT @PageSize ROWS ONLY
            ";

            var totalItems = await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdated == request.UserCodeUpdated).CountAsync();
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
            return await _context.TimeAttendanceEditHistories.Where(e => e.UserCodeUpdated == userCode && e.IsSentToHR == false).CountAsync();
        }
    }
}
