using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Application.Interfaces.Purchase;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.Approval
{
    public class ApprovalService : IApprovalService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly ITFormService _IitFormService;
        private readonly IMemoNotificationService _memoNotificationService;
        private readonly IPurchaseService _purchaseService;

        public ApprovalService
        (
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILeaveRequestService leaveRequestService,
            IMemoNotificationService memoNotificationService,
            ITFormService IitFormService,
            IPurchaseService purchaseService
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _leaveRequestService = leaveRequestService;
            _memoNotificationService = memoNotificationService;
            _IitFormService = IitFormService;
            _purchaseService = purchaseService;
        }

        public async Task<PagedResults<PendingApproval>> ListWaitApprovals(ListWaitApprovalRequest request)
        {
            var userClaims = _httpContextAccessor.HttpContext.User;

            double page = request.Page;
            double pageSize = request.PageSize;

            var roleClaims = userClaims.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var permissionClaims = userClaims.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            bool isHR = roleClaims?.Contains("HR") == true && permissionClaims?.Contains("leave_request.hr_management_leave_request") == true;

            var query = _context.ApplicationForms
                .OrderByDescending(e => e.CreatedAt)
                .Where(e => e.DeletedAt == null)
                .AsQueryable();

            if (request.RequestTypeId != null)
            {
                query = query.Where(e => e.RequestTypeId == request.RequestTypeId);
            }

            if (request.DepartmentId != null)
            {
                query = query.Where(e => e.DepartmentId == request.DepartmentId);
            }

            if (isHR)
            {
                query = query.Where(e => e.OrgPositionId == request.OrgPositionId &&
                    (
                        e.RequestStatusId == (int)StatusApplicationFormEnum.PENDING ||
                        e.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS
                    ) ||
                    e.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR
                );
            }
            else
            {
                query = query.Where(e =>
                    e.OrgPositionId == request.OrgPositionId &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                );
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var results = await query
                .Select(r => new PendingApproval
                {
                    Id = r.Id,
                    Code = r.Code,
                    UserCodeRequestor = r.UserCodeRequestor,
                    UserNameCreated = r.UserNameCreated,
                    UserNameRequestor = r.UserNameRequestor,
                    OrgUnit = r.OrgUnit,
                    CreatedAt = r.CreatedAt,
                    RequestStatus = r.RequestStatus,
                    RequestType = r.RequestType,
                    HistoryApplicationForm = r.HistoryApplicationForms
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new HistoryApplicationForm { UserNameApproval = h.UserNameApproval })
                        .FirstOrDefault(),
                })
                .Skip((int)(page - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ToListAsync();

            return new PagedResults<PendingApproval>
            {
                Data = results,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
        }

        public async Task<object> Approval(ApprovalRequest request)
        {
            if (request.RequestTypeId == null)
            {
                throw new ValidationException("Loại yêu cầu không hợp lệ");
            }

            if (request.RequestTypeId == (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION)
            {
                await _memoNotificationService.Approval(request);
            }
            else if (request.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST)
            {
                await _leaveRequestService.Approval(request);
            }
            else if (request.RequestTypeId == (int)RequestTypeEnum.FORM_IT)
            {
                await _IitFormService.Approval(request);
            }
            else if (request.RequestTypeId == (int)RequestTypeEnum.PURCHASE)
            {
                await _purchaseService.Approval(request);
            }

            return true;
        }

        public async Task<CountWaitApprovalAndAssignedInSidebarResponse> CountWaitAprrovalAndAssignedInSidebar(CountWaitAprrovalAndAssignedInSidebarRequest request)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var permissionClaims = userClaims?.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            bool isHR = roleClaims?.Contains("HR") == true && permissionClaims?.Contains("leave_request.hr_management_leave_request") == true;

            var query = _context.ApplicationForms
                .Where(e => e.DeletedAt == null)
                .OrderByDescending(e => e.CreatedAt)
                .AsQueryable();

            if (isHR)
            {
                query = query.Where(e => e.OrgPositionId == request.OrgPositionId &&
                    (
                        e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                        e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                    ) ||
                    e.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR
                );
            }
            else
            {
                query = query.Where(e =>
                    e.OrgPositionId == request.OrgPositionId &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                );
            }

            var totalItems = await query.CountAsync();

            var queryCountAssigned = await _context.ApplicationForms
                .Where(e => 
                    e.AssignedTasks.Any(at => at.UserCode == request.UserCode) && 
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                )
                .CountAsync();

            CountWaitApprovalAndAssignedInSidebarResponse results = new CountWaitApprovalAndAssignedInSidebarResponse();

            results.TotalWaitApproval = totalItems;
            results.TotalAssigned = queryCountAssigned;

            return results;
        }

        public async Task<PagedResults<HistoryApprovalProcessResponse>> ListHistoryApprovedOrProcessed(ListHistoryApprovalProcessedRequest request)
        {
            string? userCode = request.UserCode;
            double page = request.Page;
            double pageSize = request.PageSize;
            int? requestTypeId = request.RequestTypeId;
            int? departmentId = request.DepartmentId;
            int? status = request.Status;

            if (string.IsNullOrWhiteSpace(userCode))
            {
                throw new ValidationException("UserCode is required");
            }

            var query = _context.HistoryApplicationForms
                .Where(e =>
                    (
                        e.UserCodeApproval == userCode ||
                        (
                            e.ApplicationForm != null &&
                            (
                                (
                                    e.ApplicationForm.RequestTypeId == (int)RequestTypeEnum.FORM_IT || e.ApplicationForm.RequestTypeId == (int)RequestTypeEnum.PURCHASE
                                ) && 
                                e.ApplicationForm.AssignedTasks.Any(at => at.UserCode == userCode)
                            )
                        )
                    )
                    && e.DeletedAt == null
                    && e.ApplicationForm != null
                    && e.ApplicationForm.DeletedAt == null
                );

            //filter by request type
            if (requestTypeId != null)
            {
                query = query.Where(e => e.ApplicationForm != null && e.ApplicationForm.RequestTypeId == requestTypeId);
            }

            //filter by department
            if (departmentId != null)
            {
                query = query.Where(e =>
                    (e.ApplicationForm.RequestType.Id == (int)RequestTypeEnum.LEAVE_REQUEST && e.ApplicationForm.Leave.DepartmentId == request.DepartmentId) ||
                    (e.ApplicationForm.RequestType.Id == (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION && e.ApplicationForm.MemoNotification.DepartmentId == request.DepartmentId) ||
                    (e.ApplicationForm.RequestType.Id == (int)RequestTypeEnum.FORM_IT && e.ApplicationForm.ITForm.DepartmentId == request.DepartmentId) ||
                    (e.ApplicationForm.RequestType.Id == (int)RequestTypeEnum.PURCHASE && e.ApplicationForm.Purchase.DepartmentId == request.DepartmentId)
                );
            }

            //filter by status
            if (status != null)
            {
                if (status == (int)StatusApplicationFormEnum.PENDING || status == (int)StatusApplicationFormEnum.FINAL_APPROVAL)
                {
                    query = query.Where(e => e.ApplicationForm != null &&
                        (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.PENDING ||
                         e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL));
                }
                else if (status == (int)StatusApplicationFormEnum.IN_PROCESS || status == (int)StatusApplicationFormEnum.ASSIGNED)
                {
                    query = query.Where(e => e.ApplicationForm != null &&
                        (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS ||
                         e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.ASSIGNED));
                }
                else
                {
                    query = query.Where(e => e.ApplicationForm != null && e.ApplicationForm.RequestStatusId == status);
                }
            }

            var totalItems = await query
                .Select(e => e.ApplicationFormId)
                .Distinct()
                .CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var latestHistories = await query
                .GroupBy(e => e.ApplicationFormId)
                .Select(g => g.OrderByDescending(x => x.CreatedAt).Select(x => x.Id).First())
                .OrderByDescending(id => id)
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var results = await _context.HistoryApplicationForms
                .Where(e => latestHistories.Contains(e.Id))
                .Select(x => new HistoryApprovalProcessResponse
                {
                    Id = x.ApplicationForm == null ? null : x.ApplicationForm.Id,
                    Code = x.ApplicationForm == null ? null : x.ApplicationForm.Code,
                    Action = x.Action,
                    RequestStatus = x.ApplicationForm == null ? null : x.ApplicationForm.RequestStatus,
                    RequestType = x.ApplicationForm == null ? null : x.ApplicationForm.RequestType,
                    OrgUnit = x.ApplicationForm == null ? null : x.ApplicationForm.OrgUnit,
                    UserNameRequestor = x.ApplicationForm == null ? null : x.ApplicationForm.UserNameRequestor,
                    UserCodeRequestor = x.ApplicationForm == null ? null : x.ApplicationForm.UserCodeRequestor,
                    ApprovedAt = x.CreatedAt
                })
                .OrderByDescending(x => x.ApprovedAt)
                .ToListAsync();

            return new PagedResults<HistoryApprovalProcessResponse>
            {
                Data = results,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
        }

        public async Task<PagedResults<PendingApproval>> ListAssigned(ListAssignedTaskRequest request)
        {
            double page = request.Page;
            double pageSize = request.PageSize;
            int? departmentId = request.DepartmentId;
            int? requestTypeId = request.RequestTypeId;

            var query = _context.ApplicationForms
                .Where(e =>
                    e.AssignedTasks.Any(at => at.UserCode == request.UserCode) &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                )
                .AsQueryable();

            //filter by request type
            if (requestTypeId != null)
            {
                query = query.Where(e => e.RequestTypeId == requestTypeId);
            }

            //filter by department
            if (request.DepartmentId != null)
            {
                query = query.Where(e => e.DepartmentId == request.DepartmentId);
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var results = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new PendingApproval
                {
                    Id = r.Id,
                    Code = r.Code,
                    UserCodeRequestor = r.UserCodeRequestor,
                    UserNameCreated = r.UserNameCreated,
                    UserNameRequestor = r.UserNameRequestor,
                    OrgUnit = r.OrgUnit,
                    CreatedAt = r.CreatedAt,
                    RequestStatus = r.RequestStatus,
                    RequestType = r.RequestType,
                    HistoryApplicationForm = r.HistoryApplicationForms
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new HistoryApplicationForm { UserNameApproval = h.UserNameApproval })
                        .FirstOrDefault(),
                })
                .Skip((int)(page - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ToListAsync();

            return new PagedResults<PendingApproval>
            {
                Data = results,
                TotalItems = totalItems,
                TotalPages = totalPages,
            };
        }
    }
}
