using Dapper;
using System.Data;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MissTimeKeeping.Requests;
using ServicePortals.Application.Dtos.MissTimeKeeping.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.MissTimeKeeping;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using System.Security.Claims;

namespace ServicePortals.Application.Services.MissTimeKeeping
{
    public class MissTimeKeepingService : IMissTimeKeepingService
    {
        private ApplicationDbContext _context;
        private IOrgPositionService _orgPositionService;
        private readonly IConfiguration _configuration;
        private readonly ExcelService _excelService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public MissTimeKeepingService(
            ApplicationDbContext context,
            IOrgPositionService orgPositionService,
            IHttpContextAccessor httpContextAccessor,
            ExcelService excelService,
            IConfiguration configuration,
            IUserService userService
        )
        {
            _context = context;
            _orgPositionService = orgPositionService;
            _configuration = configuration;
            _excelService = excelService;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<object> Create(CreateMissTimeKeepingRequest request)
        {
            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId);

            if (orgPosition == null)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            int nextOrgPositionId = 9999;
            int statusId = (int)StatusApplicationFormEnum.PENDING;
            bool isSendHr = false;

            if (orgPosition.UnitId == (int)UnitEnum.GM || orgPosition.UnitId == (int)UnitEnum.Manager)
            {
                nextOrgPositionId = 0;
                statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                isSendHr = true;
            }
            else
            {
                var orgPositionManager = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(orgPosition.Id);
                nextOrgPositionId = orgPositionManager?.Id ?? 9999;
            }

            var newApplicationForm = new ApplicationForm
            {
                Code = Helper.GenerateFormCode("MTK"),
                RequestTypeId = (int)RequestTypeEnum.MISS_TIMEKEEPING,
                RequestStatusId = statusId,
                OrgPositionId = nextOrgPositionId,
                UserCodeCreatedForm = request?.UserCodeCreated,
                UserNameCreatedForm = request?.UserNameCreated,
                DepartmentId = request?.DepartmentId,
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
            List<Domain.Entities.MissTimeKeeping> missTimeKeepings = [];

            if (request?.ListCreateMissTimeKeepings.Count > 0)
            {
                foreach (var itemMissTimeKeeping in request.ListCreateMissTimeKeepings)
                {
                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        ApplicationForm = newApplicationForm,
                        UserCode = itemMissTimeKeeping.UserCode,
                        UserName = itemMissTimeKeeping.UserName,
                        Status = true,
                        CreatedAt = DateTimeOffset.Now
                    };
                    applicationFormItems.Add(newApplicationFormItem);

                    var newMissTimeKeeping = new Domain.Entities.MissTimeKeeping
                    {
                        ApplicationFormItem = newApplicationFormItem,
                        UserCode = itemMissTimeKeeping.UserCode,
                        UserName = itemMissTimeKeeping.UserName,
                        DateRegister = itemMissTimeKeeping.DateRegister,
                        Shift = itemMissTimeKeeping.Shift,
                        AdditionalIn = itemMissTimeKeeping.AdditionalIn,
                        AdditionalOut = itemMissTimeKeeping.AdditionalOut,
                        FacialRecognitionIn = itemMissTimeKeeping.FacialRecognitionIn,
                        FacialRecognitionOut = itemMissTimeKeeping.FacialRecognitionOut,
                        GateIn = itemMissTimeKeeping.GateIn,
                        GateOut = itemMissTimeKeeping.GateOut,
                        Reason = itemMissTimeKeeping.Reason,
                        CreatedAt = DateTimeOffset.Now
                    };
                    missTimeKeepings.Add(newMissTimeKeeping);
                }
            }

            _context.ApplicationForms.Add(newApplicationForm);
            _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
            _context.ApplicationFormItems.AddRange(applicationFormItems);
            _context.MissTimeKeepings.AddRange(missTimeKeepings);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/view-miss-timekeeping-approval/{newApplicationForm.Code}";
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
                job.SendEmailMissTimeKeeping(
                    nextUserApprovals.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for miss timekeping approval",
                    TemplateEmail.SendContentEmail("Request for miss timekeping approval", urlApproval, newApplicationForm.Code),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> Update(string applicationFormCode, CreateMissTimeKeepingRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(e => e.Code == applicationFormCode);

            if (applicationForm == null)
            {
                throw new NotFoundException("Application form not found, please check again");
            }

            applicationForm.DepartmentId = request.DepartmentId;
            applicationForm.UpdatedAt = DateTimeOffset.Now;

            _context.ApplicationForms.Update(applicationForm);

            var requestIds = request.ListCreateMissTimeKeepings
                .Where(x => x.Id.HasValue)
                .Select(x => x!.Id!.Value)
                .ToList();

            await _context.MissTimeKeepings
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
                    !afi.MissTimeKeepings.Any(ot => requestIds.Contains(ot.Id))
                )
                .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, DateTimeOffset.Now));

            var existingMissTimeKeepings = await _context.MissTimeKeepings.Where(x => requestIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id);

            foreach (var itemMissTimeKeeping in request.ListCreateMissTimeKeepings)
            {
                if (itemMissTimeKeeping.Id.HasValue && existingMissTimeKeepings.TryGetValue(itemMissTimeKeeping.Id.Value, out var missTimeKeeping))
                {
                    missTimeKeeping.UserCode = itemMissTimeKeeping.UserCode;
                    missTimeKeeping.UserName = itemMissTimeKeeping.UserName;
                    missTimeKeeping.DateRegister= itemMissTimeKeeping.DateRegister;
                    missTimeKeeping.Shift= itemMissTimeKeeping.Shift;

                    missTimeKeeping.AdditionalIn = itemMissTimeKeeping.AdditionalIn;
                    missTimeKeeping.AdditionalOut = itemMissTimeKeeping.AdditionalOut;
                    missTimeKeeping.FacialRecognitionIn = itemMissTimeKeeping.FacialRecognitionIn;
                    missTimeKeeping.FacialRecognitionOut = itemMissTimeKeeping.FacialRecognitionOut;
                    missTimeKeeping.GateIn = itemMissTimeKeeping.GateIn;
                    missTimeKeeping.GateOut = itemMissTimeKeeping.GateOut;
                    missTimeKeeping.Reason = itemMissTimeKeeping.Reason;
                    missTimeKeeping.UpdatedAt = DateTimeOffset.Now;

                    _context.MissTimeKeepings.Update(missTimeKeeping);
                }
                else //add new
                {
                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        ApplicationFormId = applicationForm.Id,
                        UserCode = itemMissTimeKeeping?.UserCode,
                        UserName = itemMissTimeKeeping?.UserName,
                        Status = true,
                        CreatedAt = DateTimeOffset.Now
                    };

                    var newMissTimeKeeping = new Domain.Entities.MissTimeKeeping
                    {
                        ApplicationFormItem = newApplicationFormItem,
                        UserCode = itemMissTimeKeeping?.UserCode,
                        UserName = itemMissTimeKeeping?.UserName,
                        AdditionalIn = itemMissTimeKeeping?.AdditionalIn,
                        AdditionalOut = itemMissTimeKeeping?.AdditionalOut,
                        FacialRecognitionIn = itemMissTimeKeeping?.FacialRecognitionIn,
                        FacialRecognitionOut = itemMissTimeKeeping?.FacialRecognitionOut,
                        GateIn = itemMissTimeKeeping?.GateIn,
                        GateOut = itemMissTimeKeeping?.GateOut,
                        Shift = itemMissTimeKeeping?.Shift,
                        Reason = itemMissTimeKeeping?.Reason,
                        CreatedAt = DateTimeOffset.Now
                    };

                    _context.ApplicationFormItems.Add(newApplicationFormItem);
                    _context.MissTimeKeepings.Add(newMissTimeKeeping);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        public async Task<PagedResults<MyMissTimeKeepingResponse>> GetMyMissTimeKeeping(MyMissTimeKeepingRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", request.Status, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<MyMissTimeKeepingResponse>(
                    "dbo.MissTimeKeeping_GET_GetMyMissTimeKeeping",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<MyMissTimeKeepingResponse>
            {
                Data = (List<MyMissTimeKeepingResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResults<MyMissTimeKeepingRegisterResponse>> GetMissTimeKeepingRegister(MyMissTimeKeepingRequest request)
        {
            var query = _context.ApplicationForms
                    .Where(e =>
                        e.UserCodeCreatedForm == request.UserCode &&
                        e.DeletedAt == null &&
                        e.RequestTypeId == (int)RequestTypeEnum.MISS_TIMEKEEPING
                    );

            if (request.Status != null)
            {
                var specialStatuses = new[]
                {
                    (int)StatusApplicationFormEnum.PENDING,
                    (int)StatusApplicationFormEnum.COMPLETE,
                    (int)StatusApplicationFormEnum.REJECT
                };

                query = specialStatuses.Contains(request.Status.Value)
                    ? query.Where(e => e.RequestStatusId == request.Status)
                    : query.Where(e => !specialStatuses.Contains(e.RequestStatusId));
            }

            var totalItem = await query.CountAsync();

            var results = await query
                .OrderByDescending(e => e.CreatedAt)
                .Include(e => e.RequestStatus)
                .Include(e => e.RequestType)
                .Select(x => new MyMissTimeKeepingRegisterResponse
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

            return new PagedResults<MyMissTimeKeepingRegisterResponse>
            {
                Data = results,
                TotalItems = totalItem,
                TotalPages = totalPages
            };
        }

        public async Task<object> Delete(string applicationFormCode)
        {
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Code == applicationFormCode)
                ?? throw new NotFoundException("Application form not found, please check again");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTimeOffset.Now;

                await _context.MissTimeKeepings
                    .Where(e => e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationForm != null && e.ApplicationFormItem.ApplicationForm.Id == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(af => af.DeletedAt, now));

                await _context.HistoryApplicationForms
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(h => h.DeletedAt, now));

                await _context.ApplicationFormItems
                    .Where(afi => afi.ApplicationFormId == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, now));

                await _context.ApplicationForms
                    .Where(af => af.Id == applicationForm.Id)
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

        public async Task<object> HrNote(HrNoteMissTimeKeepingRequest request)
        {
            var missTimeKeeping = await _context.MissTimeKeepings.FirstOrDefaultAsync(e => e.Id == request.MissTimeKeepingId);
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == request.ApplicationFormId);

            if (missTimeKeeping == null || applicationForm == null)
            {
                throw new NotFoundException("Form miss timekeeping not found, please check again");
            }

            await _context.MissTimeKeepings.Where(e => e.Id == request.MissTimeKeepingId).ExecuteUpdateAsync(s => s.SetProperty(h => h.NoteOfHR, request.NoteOfHr));

            string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/miss-timekeeping/{applicationForm.Code}";

            List<string> userCodeReceiveMail = [request.UserCode, missTimeKeeping.UserCode, applicationForm.UserCodeCreatedForm];

            var dataUserEmails = await _userService.GetMultipleUserViclockByOrgPositionId(-1, userCodeReceiveMail.Distinct().ToList());

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailMissTimeKeeping(
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

            List<string> UserCodeCreatedFormAndMissTimeKeeping = [applicationForm.UserCodeCreatedForm];
            UserCodeCreatedFormAndMissTimeKeeping.AddRange(applicationForm.ApplicationFormItems.Where(e => e.Status == true).Select(e => e.UserCode ?? "").ToList());
            UserCodeCreatedFormAndMissTimeKeeping = UserCodeCreatedFormAndMissTimeKeeping.Distinct().ToList();

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

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/miss-timekeeping/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, UserCodeCreatedFormAndMissTimeKeeping);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailMissTimeKeeping(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your miss timekeeping request has been approved",
                        TemplateEmail.SendContentEmail("Your miss timekeeping request has been approved", urlView, applicationForm.Code ?? ""),
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

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/miss-timekeeping/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, UserCodeCreatedFormAndMissTimeKeeping);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailMissTimeKeeping(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your miss timekeeping request has been rejected",
                        TemplateEmail.SendContentEmail("Your miss timekeeping request has been rejected", urlView, applicationForm.Code ?? ""),
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
                if (orgPosition.UnitId == (int)UnitEnum.GM || orgPosition.UnitId == (int)UnitEnum.Manager)
                {
                    nextOrgPositionId = 0;
                    statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                    isSendHr = true;
                }
                else
                {
                    var orgPositionManager = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(orgPosition.Id);
                    nextOrgPositionId = orgPositionManager?.Id ?? 9999;
                }
            }

            applicationForm.RequestStatusId = statusId;
            applicationForm.OrgPositionId = nextOrgPositionId;

            historyApplicationForm.Action = "Approved";

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/view-miss-timekeeping-approval/{applicationForm.Code}";

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
                job.SendEmailMissTimeKeeping(
                    nextUserApproval.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for miss timekeeping approval",
                    TemplateEmail.SendContentEmail("Request for miss timekeeping request approval", urlApproval, applicationForm.Code ?? ""),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> GetMissTimeKeepingByApplicationFormCode(string applicationFormCode)
        {
            var missTimeKeepings = await _context.MissTimeKeepings
                .Include(e => e.ApplicationFormItem)
                .Where(e =>
                    e.ApplicationFormItem != null &&
                    e.ApplicationFormItem.ApplicationForm != null &&
                    e.ApplicationFormItem.ApplicationForm.Code == applicationFormCode
                )
                .OrderByDescending(e => e!.ApplicationFormItem!.Status)
                .ToListAsync();

            foreach (var item in missTimeKeepings)
            {
                item!.ApplicationFormItem!.MissTimeKeepings = [];
            }

            var applicationForm = await _context.ApplicationForms
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
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
                missTimeKeepings,
                applicationForm
            };
        }

        public async Task<byte[]> HrExportExcel(long applicationFormId)
        {
            var applicationForm = await _context.ApplicationForms
                .Include(e => e.ApplicationFormItems)
                    .ThenInclude(e => e.MissTimeKeepings)
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == applicationFormId);

            if (applicationForm == null)
            {
                return [];
            }

            applicationForm!.ApplicationFormItems = applicationForm.ApplicationFormItems.Where(e => e.Status == true).ToList();

            return _excelService.ExportMissTimeKeepingToExcel(applicationForm);
        }
    }
}

