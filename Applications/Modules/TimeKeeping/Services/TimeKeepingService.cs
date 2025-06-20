﻿using System.Data;
using Dapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.TimeKeeping.DTO.Requests;
using ServicePortal.Applications.Modules.TimeKeeping.Services.Interfaces;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;
using ServicePortal.Infrastructure.Persistence;

namespace ServicePortal.Applications.Modules.TimeKeeping.Services
{
    public class TimeKeepingService : ITimeKeepingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDapperQueryService _dapperQueryService;
        private readonly IEmailService _emailService;

        public TimeKeepingService (IDapperQueryService dapperQueryService, IEmailService emailService, ApplicationDbContext context)
        {
            _dapperQueryService = dapperQueryService;
            _emailService = emailService;
            _context = context;
        }

        public async Task<IEnumerable<object>> GetPersonalTimeKeeping(GetPersonalTimeKeepingDto request)
        {
            var param = new
            {
                request.FromDate,
                request.ToDate,
                StaffCode = request.UserCode
            };

            var result = await _dapperQueryService.QueryAsync<object>(
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

            var rows = await connection.QueryAsync(
                "sp_GetUserAttendanceTimeKeeping", //store proceduce in database service portal
                new
                {
                    UserCodeManage = request.UserCode,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageNumber = request.Page,
                    request.PageSize
                },
                commandType: CommandType.StoredProcedure
            );

            var records = rows
                .GroupBy(r => r.NVMaNV)
                .Select(g => new
                {
                    NVMaNV = g.Key,
                    g.First().NVHoTen,
                    g.First().BPTen,
                    DataTimeKeeping = g.Select(r => new
                    {
                        r.BCNgay,
                        r.Thu,
                        r.CVietTat,
                        r.BCTGDen,
                        r.BCTGVe,
                        r.BCTGLamNgay,
                        r.BCTGLamToi,
                        r.BCGhiChu,
                        r.result
                    }).OrderBy(r => r.BCNgay).ToList()
                })
                .OrderBy(r => r.NVMaNV)
                .ToList();

            return records;
        }

        public async Task<object> ConfirmTimeKeepingToHr(GetManagementTimeKeepingRequest request)
        {
            //ManagementTimeKeepingResponseDto data = await GetManagementTimeKeeping(request);

            using var stream = new MemoryStream();
            var fileBytes = stream.ToArray(); // _excelService.GenerateExcelManagerConfirmToHR(data, request);

            BackgroundJob.Enqueue<EmailService>(job => job.SendEmailConfirmTimeKeepingToHr(fileBytes, request));

            return true;
        }
    }
}
