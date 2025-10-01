using System.Security.Claims;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.InternalMemoHR.Requests;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.InternalMemoHR;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.InternalMemoHR
{
    public class InternalMemoHrServiceImpl : InternalMemoHrService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InternalMemoHrServiceImpl(
            ApplicationDbContext context, 
            IConfiguration configuration, 
            IUserService userService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _configuration = configuration;
            _userService = userService;
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

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/internal-memo-hr/view/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, [applicationForm.UserCodeCreatedForm]);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your internal memo hr request has been approved",
                        TemplateEmail.SendContentEmail("Your internal memo hr request has been approved", urlView, applicationForm.Code ?? ""),
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

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/internal-memo-hr/view/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, [applicationForm.UserCodeCreatedForm]);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your internal memo request has been rejected",
                        TemplateEmail.SendContentEmail("Your internal memo request has been rejected", urlView, applicationForm.Code ?? ""),
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
            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.INTERNAL_MEMO_HR && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

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
                    nextOrgPositionId = orgPosition?.ParentOrgPositionId ?? 9999;
                }
            }

            applicationForm.RequestStatusId = statusId;
            applicationForm.OrgPositionId = nextOrgPositionId;

            historyApplicationForm.Action = "Approved";

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/internal-memo/{applicationForm.Code}?mode=approval";

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
                    "Request for internal memo HR approval",
                    TemplateEmail.SendContentEmail("Request for internal memo HR approval", urlApproval, applicationForm.Code ?? ""),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> Create(CreateInternalMemoHrRequest request)
        {
            int orgPositionId = request.OrgPositionId;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId)
                ?? throw new ValidationException(Global.UserNotSetInformation);

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
                nextOrgPositionId = orgPosition?.ParentOrgPositionId ?? 9999;
            }

            var metaData = new
            {
                request.Title,
                request.TitleOther,
                request.Save,
                request.Headers,
                request.Rows
            };

            var newApplicationForm = new ApplicationForm
            {
                Code = Helper.GenerateFormCode("IMHR"),
                RequestTypeId = (int)RequestTypeEnum.INTERNAL_MEMO_HR,
                RequestStatusId = statusId,
                OrgPositionId = nextOrgPositionId,
                UserCodeCreatedForm = request?.UserCodeCreated,
                UserNameCreatedForm = request?.UserNameCreated,
                DepartmentId = request?.DepartmentId, 
                Note = request?.Note,
                MetaData = JsonConvert.SerializeObject(metaData),
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

            _context.ApplicationForms.Add(newApplicationForm);
            _context.HistoryApplicationForms.Add(newHistoryApplicationForm);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/internal-memo-hr/{newApplicationForm.Code}?mode=approval";
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
                    "Request for internal memo HR approval",
                    TemplateEmail.SendContentEmail("Request for internal memo HR approval", urlApproval, newApplicationForm.Code),
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

        public async Task<ApplicationForm> GetDetailInternalMemoByApplicationFormCode(string applicationFormCode)
        {
            var applicationForm = await _context.ApplicationForms
                .Include(e => e.RequestStatus)
                .Include(e => e.RequestType)
                .Include(e => e.OrgUnit)
                .FirstOrDefaultAsync(e => e.Code == applicationFormCode)
            ?? throw new NotFoundException("Application form not found, please check again");

            applicationForm.HistoryApplicationForms = await _context.HistoryApplicationForms
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .OrderByDescending(e => e.ActionAt)
                    .AsNoTracking()
                    .ToListAsync();

            foreach (var itemHistory in applicationForm.HistoryApplicationForms)
            {
                itemHistory.ApplicationForm = null;
            }

            return applicationForm;
        }

        public async Task<PagedResults<ApplicationForm>> GetList(GetListInternalMemoHrRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.ApplicationForms.Where(e => e.UserCodeCreatedForm == request.UserCode && e.RequestTypeId == (int)RequestTypeEnum.INTERNAL_MEMO_HR).AsQueryable();

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

            query = query.OrderByDescending(e => e.CreatedAt);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var applicationForms = await query
                .Include(e => e.OrgUnit)
                .Include(e => e.RequestStatus)
                .Include(e => e.RequestType)
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var result = new PagedResults<ApplicationForm>
            {
                Data = applicationForms,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<object> Update(string applicationFormCode, CreateInternalMemoHrRequest request)
        {
            var applicationForm = await _context.ApplicationForms
                .FirstOrDefaultAsync(e => e.Code == applicationFormCode)
                ?? throw new NotFoundException("Application form not found, please check again");

            var metaData = new
            {
                request.Title,
                request.TitleOther,
                request.Save,
                request.Headers,
                request.Rows
            };

            applicationForm.DepartmentId = request.DepartmentId;
            applicationForm.Note = request.Note;
            applicationForm.MetaData = JsonConvert.SerializeObject(metaData);
            applicationForm.UpdatedAt = DateTimeOffset.Now;

            _context.ApplicationForms.Update(applicationForm);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
