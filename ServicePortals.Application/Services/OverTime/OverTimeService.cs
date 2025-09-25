using ClosedXML.Excel;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.OverTime.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.OverTime;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.OverTime
{
    public class OverTimeService : IOverTimeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public OverTimeService(
            ApplicationDbContext context, 
            IUserService userService,
            IConfiguration configuration
        )
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<object> Create(CreateOverTimeRequest request)
        {
            int orgPositionId = request.OrgPositionId;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId) 
                ?? throw new ValidationException(Global.UserNotSetInformation);

            int nextOrgPositionId = 9999;
            int statusId = (int)StatusApplicationFormEnum.PENDING;
            bool isSendHr = false;

            if (orgPosition.UnitId == (int)UnitEnum.GM || orgPosition.PositionCode == "ADMIN-MGR")
            {
                nextOrgPositionId = 0;
                statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                isSendHr = true;
            }
            else
            {
                nextOrgPositionId = orgPosition?.ParentOrgPositionId ?? 9999;
            }

            var newApplicationForm = new ApplicationForm
            {
                Code = Helper.GenerateFormCode("OT"),
                RequestTypeId = (int)RequestTypeEnum.OVERTIME,
                RequestStatusId = statusId,
                OrgPositionId = nextOrgPositionId,
                UserCodeCreatedForm = request?.UserCodeCreated,
                UserNameCreatedForm = request?.UserNameCreated,
                DepartmentId = request?.DepartmentId, //department đăng ký
                DateRegister = request?.DateRegisterOT,
                TypeOverTimeId = request?.TypeOverTimeId,
                OrgUnitCompanyId = request?.OrgUnitCompanyId,
                CreatedAt = DateTimeOffset.Now
            };

            var newHistoryApplicationForm = new HistoryApplicationForm
            {
                ApplicationForm = newApplicationForm,
                Action = "Created",
                UserCodeAction = request?.UserCodeCreated,
                UserNameAction = request?.UserNameCreated,
                ActionAt = DateTimeOffset.Now,
            };

            List<ApplicationFormItem> applicationFormItems = [];
            List<Domain.Entities.OverTime> overTimes = [];

            if (request?.CreateListOverTimeRequests.Count > 0)
            {
                foreach (var itemOt in request.CreateListOverTimeRequests)
                {
                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        ApplicationForm = newApplicationForm,
                        UserCode = itemOt.UserCode,
                        UserName = itemOt.UserName,
                        Status = true,
                        CreatedAt = DateTimeOffset.Now
                    };
                    applicationFormItems.Add(newApplicationFormItem);

                    var newOtItem = new Domain.Entities.OverTime
                    {
                        ApplicationFormItem = newApplicationFormItem,
                        UserCode = itemOt.UserCode,
                        UserName = itemOt.UserName,
                        Position = itemOt.Position,
                        FromHour = itemOt.FromHour,
                        ToHour = itemOt.ToHour,
                        NumberHour = itemOt.NumberHour,
                        Note = itemOt.Note,
                        CreatedAt = DateTimeOffset.Now
                    };
                    overTimes.Add(newOtItem);
                }
            }
            else //excel
            {
                using var workbook = new XLWorkbook(request?.File?.OpenReadStream());
                var worksheet = workbook.Worksheet(1);

                var resultApplicationFormItemAndOverTime = await ValidateExcel(worksheet, newApplicationForm);

                applicationFormItems = resultApplicationFormItemAndOverTime.applicationFormItems;
                overTimes = resultApplicationFormItemAndOverTime.overTime;

                if (applicationFormItems.Count <= 0 || overTimes.Count <= 0)
                {
                    throw new ValidationException("Dữ liệu nhập vào không hợp lệ");
                }
            }

            _context.ApplicationForms.Add(newApplicationForm);
            _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
            _context.ApplicationFormItems.AddRange(applicationFormItems);
            _context.OverTimes.AddRange(overTimes);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/overtime/approval/{newApplicationForm.Code}";
            List<GetMultiUserViClockByOrgPositionIdResponse> nextUserApprovals = [];

            if (isSendHr)
            {
                var permissionHrMngLeaveRequest = await _context.Permissions
                    .Include(e => e.UserPermissions)
                    .FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

                nextUserApprovals = await _userService.GetMultipleUserViclockByOrgPositionId(-1, permissionHrMngLeaveRequest?.UserPermissions?.Select(e => e.UserCode ?? "")?.ToList());
            }
            else
            {
                nextUserApprovals = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId);
            }

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailAsync(
                    nextUserApprovals.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for overtime approval",
                    TemplateEmail.SendContentEmail("Request for overtime approval", urlApproval, newApplicationForm.Code),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<List<TypeOverTime>> GetAllTypeOverTime()
        {
            return await _context.TypeOverTimes.ToListAsync();
        }

        private async Task<(List<ApplicationFormItem> applicationFormItems, List<Domain.Entities.OverTime> overTime)> ValidateExcel(IXLWorksheet worksheet,ApplicationForm newApplicationForm)
        {
            try
            {
                List<ApplicationFormItem> applicationFormItems = [];
                List<Domain.Entities.OverTime> overTimes = [];

                Helper.ValidateExcelHeader(worksheet, ["Mã Nhân Viên", "Họ Tên", "Chức Vụ"]);

                var rows = (worksheet?.RangeUsed()?.RowsUsed().Skip(3)) ?? throw new ValidationException("Không có dữ liệu nào, kiểm tra lại file excel");

                int currentRow = 4;

                foreach (var row in rows)
                {
                    string userCode = row.Cell(1).GetValue<string>();
                    string userName = row.Cell(2).GetValue<string>();
                    string position = row.Cell(3).GetValue<string>();
                    string fromHour = row.Cell(4).GetValue<string>();
                    string toHour = row.Cell(5).GetValue<string>();
                    int numberHour = row.Cell(6).GetValue<int>();
                    string note = row.Cell(7).GetValue<string>();

                    bool isEmptyRow = string.IsNullOrWhiteSpace(userCode)
                        && string.IsNullOrWhiteSpace(userName)
                        && string.IsNullOrWhiteSpace(position)
                        && string.IsNullOrWhiteSpace(fromHour)
                        && string.IsNullOrWhiteSpace(toHour)
                        && numberHour == 0
                        && string.IsNullOrWhiteSpace(note);

                    if (isEmptyRow)
                    {
                        break;
                    }

                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        ApplicationForm = newApplicationForm,
                        UserCode = userCode,
                        UserName = userName,
                        Status = true,
                        CreatedAt = DateTimeOffset.Now
                    };

                    applicationFormItems.Add(newApplicationFormItem);

                    var newOverTime = new Domain.Entities.OverTime
                    {
                        ApplicationFormItem = newApplicationFormItem,
                        UserCode = userCode,
                        UserName = userName,
                        Position = position,
                        FromHour = fromHour,
                        ToHour = toHour,
                        NumberHour = numberHour,
                        Note = note,
                        CreatedAt = DateTimeOffset.Now
                    };

                    overTimes.Add(newOverTime);
                    currentRow++;
                }

                return (applicationFormItems, overTimes);
            }
            catch (Exception ex)
            {
                throw new ValidationException("Dữ liệu nhập vào không hợp lệ");
            }
        }
    }
}
