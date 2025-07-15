using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Requests;
using ServicePortals.Application.Dtos.TimeKeeping.Responses;
using ServicePortals.Application.Interfaces.TimeKeeping;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<PagedResults<GroupedUserTimeKeeping>> GetManagementTimeKeeping(GetManagementTimeKeepingRequest request)
        {
            int month = request.Month ?? DateTime.UtcNow.Month;
            int year = request.Year ?? DateTime.UtcNow.Year;
            int dayInMonth = DateTime.DaysInMonth(year, month);
            double pageSize = request.PageSize;
            double page = request.Page;

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
                    FROM user_mng_org_unit_id as um
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

            var countUser = await connection.QuerySingleAsync<int>($@"SELECT COUNT(*) FROM vs_new.dbo.tblNhanVien AS NV WHERE NV.OrgUnitID IN @ids", new { ids = orgUnitIds });

            var totalPages = (int)Math.Ceiling((double)countUser / pageSize);

            var sql = $@"
                WITH Users AS (
	                SELECT
		                NV.NVMa, NV.NVMaNV, vs_new.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen, BP.BPTen
	                FROM vs_new.dbo.tblNhanVien AS NV
	                LEFT JOIN vs_new.dbo.tblBoPhan AS BP ON NV.NVMaBP = BP.BPMa
	                WHERE NV.OrgUnitID IN @ids --'13136', '17024', '16239', 
	                ORDER BY NV.NVMa ASC
	                OFFSET (@Page - 1) * @PageSize ROWS
	                FETCH NEXT @PageSize ROWS ONLY
                )
                SELECT 
	                U.NVMaNV,
	                U.NVHoTen,
	                U.BPTen,
	                CASE WHEN DATEPART(dw, BCNGay) = 1 THEN 'CN' ELSE convert(nvarchar(10),DATEPART(dw, BCNGay)) END AS Thu,
	                CONVERT(VARCHAR(10), BC.BCNgay, 120) AS BCNgay,
	                BC.BCTGDen,
	                BC.BCTGVe,
                    BC.BCGhiChu,
	                CASE
		                --Chủ nhật
		                WHEN DATEPART(dw, BCNGay) = 1 THEN
			                CASE
				                WHEN BCTGDen IS NOT NULL AND BCTGVe IS NOT NULL  THEN 'CN_X'
			                END

		                -- ngày thường
		                ELSE
			                CASE
				                WHEN BCGhiChu != '' THEN BCGhiChu

				                WHEN BCTGLamNgay + BCTGLamToi = BCTGQuyDinh THEN 'X'
					
				                WHEN BCTGDen IS NOT NULL AND BCTGVe IS NOT NULL THEN 
					                CASE 
						                WHEN 1.0 * CEILING(1.0 * (BCTGQuyDinh - (BCTGLamNgay + BCTGLamToi)) / 30.0) * 30 
							                    / NULLIF(BCTGQuyDinh, 0) = 1 THEN '?'
						                ELSE RTRIM(
								                FORMAT(
										                1.0 * CEILING(1.0 * (BCTGQuyDinh - (BCTGLamNgay + BCTGLamToi)) / 30.0) * 30 
										                / NULLIF(BCTGQuyDinh, 0),
										                '0.####'
									                )
							                )
					                END
				                ELSE
					                '?'
			                END
	                END AS Results
	                --BC.*
                FROM Users AS U
                LEFT JOIN vs_new.dbo.tblBaoCao AS BC ON BC.BCMaNV = U.NVMa
                WHERE BCNgay BETWEEN @FromDate AND @ToDate
                ORDER BY U.NVMaNV ASC
            ";

            var param = new
            {
                Page = (int)page,
                PageSize = (int)pageSize,
                Month = month,
                Year = year,
                FromDate = fromDate,
                ToDate = toDate,
                ids = orgUnitIds
            };

            var result = (await connection.QueryAsync<GetUserTimeKeepingResponse>(sql, param)).ToList();

            var groupedResult = result
                .GroupBy(x => new { x.NVMaNV, x.NVHoTen, x.BPTen })
                .Select(g => new GroupedUserTimeKeeping
                {
                    NVMaNV = g.Key.NVMaNV,
                    NVHoTen = g.Key.NVHoTen,
                    BPTen = g.Key.BPTen,
                    DataTimeKeeping = g.Select(x => new UserDailyRecord
                    {
                        thu = x.Thu,
                        bcNgay = x.BCNgay,
                        vao = x.BCTGDen,
                        ra = x.BCTGVe,
                        result = x.Results,
                        bcGhiChu = x.BCGhiChu
                    }).ToList()
            }   ).ToList();

            var finalResult = new PagedResults<GroupedUserTimeKeeping>
            {
                Data = groupedResult,
                TotalItems = groupedResult.Count,
                TotalPages = totalPages
            };

            return finalResult;

            //return groupedResult;
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
        public async Task<object> UpdateUserMngTimeKeeping(UpdateUserMngTimeKeepingRequest request)
        {
            var oldData = await _context.UserMngOrgUnits.Where(e => e.UserCode == request.UserCode).ToListAsync();

            _context.UserMngOrgUnits.RemoveRange(oldData);

            List<UserMngOrgUnitId> umt = [];

            foreach (var orgUnitId in request.OrgUnitId)
            {
                umt.Add(new UserMngOrgUnitId
                {
                    UserCode = request.UserCode,
                    OrgUnitId = orgUnitId
                });
            }

            _context.UserMngOrgUnits.AddRange(umt);

            await _context.SaveChangesAsync();

            return true;
        }
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

        public async Task<object> GetOrgUnitIdAttachedByUserCode(string userCode)
        {
            var results = await _context.UserMngOrgUnits
                .Where(e => e.UserCode == userCode && e.ManagementType == "MNG_TIME_KEEPING")
                .Select(e => e.OrgUnitId)
                .ToListAsync();

            return results;
        }

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
    }
}
