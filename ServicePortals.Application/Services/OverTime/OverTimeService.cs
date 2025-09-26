using System.Data;
using System.Security.Claims;
using ClosedXML.Excel;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.OverTime.Requests;
using ServicePortals.Application.Dtos.OverTime.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.OverTime;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using Z.EntityFramework.Plus;

namespace ServicePortals.Application.Services.OverTime
{
    public class OverTimeService : IOverTimeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ExcelService _excelService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OverTimeService(
            ApplicationDbContext context, 
            IUserService userService,
            IConfiguration configuration,
            ExcelService excelService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _userService = userService;
            _configuration = configuration;
            _excelService = excelService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<object> Approval(ApprovalRequest request)
        {
            var userClaim = _httpContextAccessor.HttpContext.User;
            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (request.OrgPositionId <= 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            var applicationForm = await _context.ApplicationForms
                .Include(e => e.ApplicationFormItems)
                .FirstOrDefaultAsync(e => e.Id == request.ApplicationFormId)
                ?? throw new NotFoundException("Application form not found, please check again");

            var todayDate = DateTimeOffset.Now.Date;

            //delegation
            var isDelegation = await _context.Delegations
                .AnyAsync(e =>
                    e.FromOrgPositionId == applicationForm.OrgPositionId &&
                    e.IsActive == true &&
                    e.UserCodeDelegation == request.UserCodeApproval &&
                    todayDate >= e.StartDate!.Value.Date && todayDate <= e.EndDate!.Value.Date
                );

            var orgPosition = await _context.OrgPositions
                .FirstOrDefaultAsync(e => e.Id == (isDelegation ? applicationForm.OrgPositionId : request.OrgPositionId))
                ?? throw new ValidationException(Global.UserNotSetInformation);

            string userNameAction = request?.UserNameApproval ?? "";
            if (isDelegation && applicationForm.OrgPositionId != request?.OrgPositionId)
            {
                userNameAction = $"{userNameAction} (Delegated)";
            }

            var historyApplicationForm = new HistoryApplicationForm
            {
                ApplicationFormId = applicationForm.Id,
                Note = request?.Note,
                UserCodeAction = request?.UserCodeApproval,
                UserNameAction = userNameAction,
                ActionAt = DateTimeOffset.Now
            };

            List<string> UserCodeCreatedFormAndOverTime = [applicationForm.UserCodeCreatedForm];
            UserCodeCreatedFormAndOverTime.AddRange(applicationForm.ApplicationFormItems.Where(e => e.Status == true).Select(e => e.UserCode ?? "").ToList());
            UserCodeCreatedFormAndOverTime = UserCodeCreatedFormAndOverTime.Distinct().ToList();

            //case hr approved
            if (request?.Status == true && applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR)
            {
                var userPermissionHrMngLeave = await _context.Permissions.Include(e => e.UserPermissions).FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");
                var userCodes = userPermissionHrMngLeave?.UserPermissions?.Select(e => e.UserCode).ToList();

                if (userCodes!.Contains(request.UserCodeApproval) == false)
                {
                    throw new ValidationException(Global.NotPermissionApproval);
                }

                applicationForm.OrgPositionId = 0;
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
                applicationForm.UpdatedAt = DateTimeOffset.Now;
                _context.ApplicationForms.Update(applicationForm);

                historyApplicationForm.Action = "Approved";
                _context.HistoryApplicationForms.Add(historyApplicationForm);
                await _context.SaveChangesAsync();

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/overtime/view/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, UserCodeCreatedFormAndOverTime);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your overtime request has been approved",
                        TemplateEmail.SendContentEmail("Your overtime request has been approved", urlView, applicationForm.Code ?? ""),
                        null,
                        true
                    )
                );

                return true;
            }

            //validate nếu như đơn này k phải là của người này duyệt và k được ủy quyền duyệt đơn
            if (applicationForm.OrgPositionId != request?.OrgPositionId && isDelegation == false)
            {
                throw new ValidationException(Global.NotPermissionApproval);
            }

            //case reject
            if (request?.Status == false)
            {
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
                applicationForm.UpdatedAt = DateTimeOffset.Now;
                _context.ApplicationForms.Update(applicationForm);

                historyApplicationForm.Action = "Reject";
                _context.HistoryApplicationForms.Add(historyApplicationForm);
                await _context.SaveChangesAsync();

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/overtime/view/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, UserCodeCreatedFormAndOverTime);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your overtime request has been rejected",
                        TemplateEmail.SendContentEmail("Your overtime request has been rejected", urlView, applicationForm.Code ?? ""),
                        null,
                        true
                    )
                );

                return true;
            }

            int statusId = 9999;
            int nextOrgPositionId = 9999;
            bool isSendHr = false;

            //lấy danh sách workflow của người hiện tại, check xem user có custom workflow không
            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

            if (approvalFlows != null)
            {
                statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                nextOrgPositionId = approvalFlows.ToOrgPositionId ?? 9999;
            }
            else
            {
                if (orgPosition.UnitId == (int)UnitEnum.GM)
                {
                    isSendHr = true;
                    statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                    nextOrgPositionId = 0;
                }
                else if (orgPosition.UnitId == (int)UnitEnum.Manager)
                {
                    if (orgPosition.PositionCode == "ADMIN-MGR")
                    {
                        isSendHr = true;
                        statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                        nextOrgPositionId = 0;
                    }
                    else
                    {
                        statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                        nextOrgPositionId = orgPosition.ParentOrgPositionId ?? 9999;
                    }
                }
                else
                {
                    statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                    nextOrgPositionId = orgPosition.ParentOrgPositionId ?? 9999;
                }
            }

            applicationForm.RequestStatusId = statusId;
            applicationForm.OrgPositionId = nextOrgPositionId;

            historyApplicationForm.Action = "Approved";

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/overtime/approval/{applicationForm.Code}";

            List<GetMultiUserViClockByOrgPositionIdResponse> nextUserApproval = [];
            if (isSendHr)
            {
                var userCodeHrMngLeaveRq = await _context.Permissions.Include(e => e.UserPermissions).FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");
                nextUserApproval = await _userService.GetMultipleUserViclockByOrgPositionId(-1, userCodeHrMngLeaveRq?.UserPermissions?.Select(e => e.UserCode ?? "").ToList());
            }
            else
            {
                nextUserApproval = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId);
            }

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailRequestHasBeenSent(
                    nextUserApproval.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for overtime request approval",
                    TemplateEmail.SendContentEmail("Request for overtime request approval", urlApproval, applicationForm.Code ?? ""),
                    null,
                    true
                )
            );

            return true;
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

                var resultApplicationFormItemAndOverTime = ValidateExcel(worksheet, newApplicationForm);

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

        public async Task<object> Delete(string applicationFormCode)
        {
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Code == applicationFormCode)
                ?? throw new NotFoundException("Application form not found, please check again");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTimeOffset.Now;

                await _context.HistoryApplicationForms
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(h => h.DeletedAt, now));

                await _context.ApplicationFormItems
                    .Where(afi => afi.ApplicationFormId == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, now));

                await _context.ApplicationForms
                    .Where(af => af.Id == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(af => af.DeletedAt, now));

                await _context.OverTimes
                    .Where(e => e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationForm != null && e.ApplicationFormItem.ApplicationForm.Id == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(af => af.DeletedAt, now));

                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            throw new Exception("Delete failed, please try again");
        }

        public async Task<List<TypeOverTime>> GetAllTypeOverTime()
        {
            return await _context.TypeOverTimes.ToListAsync();
        }

        public async Task<object> GetDetailOverTimeByApplicationFormCode(string applicationFormCode)
        {
            var overTimes = await _context.OverTimes
                .Include(e => e.ApplicationFormItem)
                .Where(e =>
                    e.ApplicationFormItem != null &&
                    e.ApplicationFormItem.ApplicationForm != null &&
                    e.ApplicationFormItem.ApplicationForm.Code == applicationFormCode
                )
                .OrderByDescending(e => e!.ApplicationFormItem!.Status)
                .ToListAsync();

            foreach (var itemOt in overTimes)
            {
                itemOt!.ApplicationFormItem!.OverTimes = [];
            }

            var applicationForm = await _context.ApplicationForms
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
                .Include(e => e.OrgUnitCompany)
                .Include(e => e.TypeOverTime)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == applicationFormCode);

            if (applicationForm != null)
            {
                applicationForm.HistoryApplicationForms = await _context.HistoryApplicationForms
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .OrderByDescending(e => e.ActionAt)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return new
            {
                overTimes,
                applicationForm
            };
        }

        public async Task<PagedResults<MyOverTimeResponse>> GetMyOverTime(MyOverTimeRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", request.Status, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<MyOverTimeResponse>(
                    "dbo.OverTime_GET_GetMyOverTime",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<MyOverTimeResponse>
            {
                Data = (List<MyOverTimeResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResults<MyOverTimeRegisterResponse>> GetOverTimeRegister(MyOverTimeRequest request)
        {
            var query = _context.ApplicationForms
                .Where(e =>
                    e.UserCodeCreatedForm == request.UserCode &&
                    e.DeletedAt == null &&
                    e.RequestTypeId == (int)RequestTypeEnum.OVERTIME
                );

            if (request.Status != null)
            {
                query = query.Where(e => e.RequestStatusId == request.Status);
            }

            var totalItem = await query.CountAsync();

            var results = await query
                .OrderByDescending(e => e.CreatedAt)
                .Include(e => e.RequestStatus)
                .Include(e => e.RequestType)
                .Select(x => new MyOverTimeRegisterResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    UserNameCreatedForm = x.UserNameCreatedForm,
                    CreatedAt = x.CreatedAt,
                    RequestStatus = x.RequestStatus,
                    RequestType = x.RequestType
                })
                .Skip((int)((request.Page - 1) * request.PageSize)).Take((int)request.PageSize)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling((double)totalItem / request.PageSize);
            var totalItems = await query.CountAsync();

            return new PagedResults<MyOverTimeRegisterResponse>
            {
                Data = results,
                TotalItems = totalItem,
                TotalPages = totalPages
            };
        }

        public async Task<byte[]> HrExportExcel(long applicationFormId)
        {
            var overTimeDatas = await _context.OverTimes
                .Where(e =>
                    e.ApplicationFormItem != null && e.ApplicationFormItem.Status == true &&
                    e.ApplicationFormItem.ApplicationForm != null && e.ApplicationFormItem.ApplicationForm.Id == applicationFormId
            )
            .ToListAsync();

            return _excelService.ExportOverTimeToExcel(overTimeDatas);
        }

        public async Task<object> HrNote(HrNoteOverTimeRequest request)
        {
            var overTime = await _context.OverTimes.FirstOrDefaultAsync(e => e.Id == request.OverTimeId);
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == request.ApplicationFormId);

            if (overTime == null || applicationForm == null)
            {
                throw new NotFoundException("Form overtime not found, please check again");
            }

            await _context.OverTimes.Where(e => e.Id == request.OverTimeId).ExecuteUpdateAsync(s => s.SetProperty(h => h.NoteOfHR, request.NoteOfHr));

            string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/overtime/view/{applicationForm.Code}";

            List<string> userCodeReceiveMail = [request.UserCode, overTime.UserCode, applicationForm.UserCodeCreatedForm];

            var dataUserEmails = await _userService.GetMultipleUserViclockByOrgPositionId(-1, userCodeReceiveMail.Distinct().ToList());

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailRequestHasBeenSent(
                    dataUserEmails.Where(e => e.NVMaNV != request.UserCode).Select(e => e.Email ?? "").ToList(), //to email
                    dataUserEmails.Where(e => e.NVMaNV == request.UserCode).Select(e => e.Email ?? "").ToList(), //cc email
                    "HR Note",
                    TemplateEmail.SendContentEmail(request.NoteOfHr ?? "Note of HR", urlView, applicationForm.Code ?? ""),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> RejectSomeOverTimes(RejectSomeOverTimeRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Code == request.ApplicationFormCode)
                ?? throw new NotFoundException("Application form not found, please check again");

                if (applicationForm.OrgPositionId != request.OrgPositionId)
                {
                    throw new ValidationException(Global.NotPermissionApproval);
                }

                var todayDate = DateTimeOffset.Now.Date;

                //delegation
                var isDelegation = await _context.Delegations
                    .AnyAsync(e =>
                        e.FromOrgPositionId == applicationForm.OrgPositionId &&
                        e.IsActive == true &&
                        e.UserCodeDelegation == request.UserCodeReject &&
                        todayDate >= e.StartDate!.Value.Date && todayDate <= e.EndDate!.Value.Date
                    );

                if (applicationForm.OrgPositionId != request.OrgPositionId && isDelegation == false)
                {
                    throw new ValidationException(Global.NotPermissionApproval);
                }

                var overTimes = await _context.OverTimes
                    .Where(e => request.OverTimeIds.Contains(e.Id))
                    .ToListAsync();

                await _context.ApplicationFormItems
                    .Where(e => overTimes.Select(l => l.ApplicationFormItemId).Contains(e.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(afi => afi.RejectedAt, DateTimeOffset.Now)
                        .SetProperty(afi => afi.Note, request.Note)
                        .SetProperty(afi => afi.Status, false)
                    );

                var newHistoryApplicationForm = new HistoryApplicationForm
                {
                    ApplicationFormId = applicationForm.Id,
                    Note = $@"{request.Note} __Reject: {string.Join(", ", overTimes.Select(l => l.UserName))}",
                    Action = "Reject",
                    UserCodeAction = request.UserCodeReject,
                    UserNameAction = request.UserNameReject,
                    ActionAt = DateTimeOffset.Now
                };

                _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/leave/view/{applicationForm.Code}";
                var userReceivedEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, overTimes.Select(e => e.UserCode ?? "").ToList());

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailAsync(
                        userReceivedEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your overtime request has been reject",
                        TemplateEmail.SendContentEmail("Your overtime request has been reject", urlView, applicationForm.Code ?? ""),
                        null,
                        true
                    )
                );

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Save failed, please check again");
            }
        }

        public async Task<object> Update(string applicationFormCode, CreateOverTimeRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(e => e.Code == applicationFormCode);

            if (applicationForm == null)
            {
                throw new NotFoundException("Application form not found, please check again");
            }

            applicationForm.DepartmentId = request.DepartmentId;
            applicationForm.OrgUnitCompanyId = request.OrgUnitCompanyId;
            applicationForm.TypeOverTimeId = request.TypeOverTimeId;
            applicationForm.DateRegister = request.DateRegisterOT;
            applicationForm.UpdatedAt = DateTimeOffset.Now;

            _context.ApplicationForms.Update(applicationForm);

            var requestIds = request.CreateListOverTimeRequests
                .Where(x => x.Id.HasValue)
                .Select(x => x!.Id!.Value)
                .ToList();

            await _context.OverTimes
                .Where(e => 
                    e.ApplicationFormItem != null && 
                    e.ApplicationFormItem.ApplicationForm != null && 
                    e.ApplicationFormItem.ApplicationForm.Code == applicationFormCode &&
                    !requestIds.Contains(e.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationFormItems
                .Where(afi => 
                    afi.ApplicationForm != null &&
                    afi.ApplicationForm.Code == applicationFormCode &&
                    !afi.OverTimes.Any(ot => requestIds.Contains(ot.Id))
                )
                .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, DateTimeOffset.Now));

            var existingOverTimes = await _context.OverTimes.Where(x => requestIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

            foreach (var itemUpdateOverTime in request.CreateListOverTimeRequests)
            {
                if (itemUpdateOverTime.Id.HasValue && existingOverTimes.TryGetValue(itemUpdateOverTime.Id.Value, out var overTime))
                {
                    overTime.UserCode = itemUpdateOverTime.UserCode;
                    overTime.UserName = itemUpdateOverTime.UserName;
                    overTime.Position = itemUpdateOverTime.Position;
                    overTime.FromHour = itemUpdateOverTime.FromHour;
                    overTime.ToHour = itemUpdateOverTime.ToHour;
                    overTime.NumberHour = itemUpdateOverTime.NumberHour;
                    overTime.Note = itemUpdateOverTime.Note;
                    overTime.UpdatedAt = DateTimeOffset.Now;

                    _context.OverTimes.Update(overTime);
                }
                else //add new
                {
                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        ApplicationForm = applicationForm,
                        UserCode = itemUpdateOverTime?.UserCode,
                        UserName = itemUpdateOverTime?.UserName,
                        Status = true,
                        CreatedAt = DateTimeOffset.Now
                    };

                    var newOverTime = new Domain.Entities.OverTime
                    {
                        ApplicationFormItem = newApplicationFormItem,
                        UserCode = itemUpdateOverTime?.UserCode,
                        UserName = itemUpdateOverTime?.UserName,
                        Position = itemUpdateOverTime?.Position,
                        FromHour = itemUpdateOverTime?.FromHour,
                        ToHour = itemUpdateOverTime?.ToHour,
                        NumberHour = itemUpdateOverTime?.NumberHour ?? 0,
                        Note = itemUpdateOverTime?.Note,
                        CreatedAt = DateTimeOffset.Now
                    };

                    _context.ApplicationFormItems.Add(newApplicationFormItem);
                    _context.OverTimes.Add(newOverTime);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private (List<ApplicationFormItem> applicationFormItems, List<Domain.Entities.OverTime> overTime) ValidateExcel(IXLWorksheet worksheet,ApplicationForm newApplicationForm)
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
                Log.Error($"Error can not save data by excel, ex: {ex.Message}");
                throw new ValidationException("Dữ liệu nhập vào không hợp lệ");
            }
        }
    }
}
