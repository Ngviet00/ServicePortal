using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Application.Interfaces.TimeKeeping;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;

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

        public async Task<IEnumerable<dynamic>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request)
        {
            int month = request.Month ?? DateTime.UtcNow.Month;
            int year = request.Year ?? DateTime.UtcNow.Year;
            int dayInMonth = DateTime.DaysInMonth(year, month);

            var fromDate = $"{year}-{month:D2}-01";
            var toDate = $"{year}-{month:D2}-{dayInMonth}";

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var orgUnitIdQuery = $@"
                WITH RecursiveOrg AS (
                    SELECT o.id
                    FROM user_mng_org_unit_time_keeping as um
                    INNER JOIN [{Global.DbViClock}].dbo.OrgUnits as o
                        ON um.OrgUnitId = o.id
                    WHERE um.UserCode = {request.UserCode}

                    UNION ALL

                    SELECT o.id
                    FROM vs_new.dbo.OrgUnits o
                    INNER JOIN RecursiveOrg ro ON o.ParentOrgUnitId = ro.id
                )
                SELECT DISTINCT id FROM RecursiveOrg
            ";

            var orgUnitIds = await connection.QueryAsync<int>(orgUnitIdQuery);

            var employeeQuery = @"
                SELECT NV.NVMaNV, vs_new.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen
                FROM vs_new.dbo.tblNhanVien AS NV
                WHERE NV.OrgUnitId IN @ids";

            var result = await connection.QueryAsync<object>(employeeQuery, new { ids = orgUnitIds });

            return result;

            //var rows = await connection.QueryAsync(
            //    "sp_GetUserAttendanceTimeKeeping", //store proceduce in database service portal
            //    new
            //    {
            //        UserCodeManage = request.UserCode,
            //        FromDate = fromDate,
            //        ToDate = toDate,
            //        PageNumber = request.Page,
            //        request.PageSize
            //    },
            //    commandType: CommandType.StoredProcedure
            //);

            //var records = rows
            //    .GroupBy(r => r.NVMaNV)
            //    .Select(g => new
            //    {
            //        NVMaNV = g.Key,
            //        g.First().NVHoTen,
            //        g.First().BPTen,
            //        DataTimeKeeping = g.Select(r => new
            //        {
            //            r.BCNgay,
            //            r.Thu,
            //            r.CVietTat,
            //            r.BCTGDen,
            //            r.BCTGVe,
            //            r.BCTGLamNgay,
            //            r.BCTGLamToi,
            //            r.BCGhiChu,
            //            r.result
            //        }).OrderBy(r => r.BCNgay).ToList()
            //    })
            //    .OrderBy(r => r.NVMaNV)
            //    .ToList();

            //return records;
        }

        public async Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request)
        {
            //using var stream = new MemoryStream();
            //var fileBytes = stream.ToArray();

            //string subject = $"Production - Confirm Attendance List [{request.Month} - {request.Year}]";
            //string bodyMail = $@"Dear HR Team, Please find attached the excel file containing the staff attendance list [{request.Month} - {request.Year}]";
            //var emailHrMngTimeKeeping = await _hrManagementService.GetEmailHRByType("MANAGE_TIMEKEEPING");

            //var attachments = new List<(string, byte[])>
            //{
            //    ($"Confirm_Attendance_T{request.Month} - {request.Year}.xlsx", fileBytes)
            //};

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
    }
}
