using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.MemoNotification;
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

        public ApprovalService
        (
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILeaveRequestService leaveRequestService,
            IMemoNotificationService memoNotificationService,
            ITFormService IitFormService
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _leaveRequestService = leaveRequestService;
            _memoNotificationService = memoNotificationService;
            _IitFormService = IitFormService;
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

            if (isHR)
            {
                query = query.Where(e => e.OrgPositionId == request.OrgPositionId &&
                    (
                        e.RequestStatusId == (int)StatusApplicationFormEnum.PENDING ||
                        e.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS
                    ) ||
                    e.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR
                );

                if (request.DepartmentId != null)
                {
                    query = query.Where(e => _context.LeaveRequests.Any(lr => lr.ApplicationFormId == e.Id && lr.DepartmentId == request.DepartmentId));
                }
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
                .Skip((int)(page - 1) * (int)pageSize)
                .Take((int)pageSize)
                .Select(r => new PendingApproval
                {
                    Id = r.Id,
                    RequestTypeId = r.RequestTypeId,
                    OrgPositionId = r.OrgPositionId,
                    CreatedAt = r.CreatedAt,
                    RequestType = r.RequestType,
                    HistoryApplicationForm = r.HistoryApplicationForms
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new HistoryApplicationForm { UserNameApproval = h.UserNameApproval })
                        .FirstOrDefault(),
                    LeaveRequest = r.Leave == null ? null : new CommonDataPendingApproval
                    {
                        Id = r.Leave.Id,
                        Code = r.Leave.Code,
                        UserNameRequestor = r.Leave.UserNameRequestor,
                        UserNameCreated = r.Leave.CreatedBy,
                        DepartmentName = r.Leave.OrgUnit == null ? "" : r.Leave.OrgUnit.Name
                    },
                    MemoNotification = r.MemoNotification == null ? null : new CommonDataPendingApproval
                    {
                        Id = r.MemoNotification.Id,
                        Code = r.MemoNotification.Code,
                        UserNameRequestor = r.MemoNotification.CreatedBy,
                        UserNameCreated = r.MemoNotification.CreatedBy,
                        DepartmentName = r.MemoNotification.OrgUnit == null ? "" : r.MemoNotification.OrgUnit.Name
                    },
                    ITForm = r.ITForm == null ? null : new CommonDataPendingApproval
                    {
                        Id = r.ITForm.Id,
                        Code = r.ITForm.Code,
                        UserNameRequestor = r.ITForm.UserNameRequestor,
                        UserNameCreated = r.ITForm.UserNameCreated,
                        DepartmentName = r.ITForm.OrgUnit == null ? "" : r.ITForm.OrgUnit.Name
                    }
                })
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

            if (string.IsNullOrWhiteSpace(userCode))
            {
                throw new ValidationException("UserCode is required");
            }

            var query = _context.HistoryApplicationForms
                .Where(e => 
                    (e.UserCodeApproval == userCode || 
                        (
                            e.ApplicationForm != null &&
                            e.ApplicationForm.RequestTypeId == (int)RequestTypeEnum.FORM_IT && 
                            e.ApplicationForm.AssignedTasks.Any(at => at.UserCode == userCode))
                        ) && 
                    e.DeletedAt == null && e.ApplicationForm != null &&
                    e.ApplicationForm.DeletedAt == null
            );

            if (requestTypeId != null)
            {
                query = query.Where(e => e.ApplicationForm != null && e.ApplicationForm.RequestTypeId == requestTypeId);
            }

            var totalItems = await query.GroupBy(e => e.ApplicationFormId).CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var pagedIds = await query
                .OrderByDescending(e => e.CreatedAt)
                .GroupBy(e => e.ApplicationFormId)
                .Select(g => g.FirstOrDefault().Id)
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var results = await _context.HistoryApplicationForms
                .Where(e => pagedIds.Contains(e.Id))
                .Select(x => new HistoryApprovalProcessResponse
                {
                    CreatedAt = x.CreatedAt,
                    Action = x.Action,
                    RequestStatusId = x.ApplicationForm != null ? x.ApplicationForm.RequestStatusId : null,
                    RequestType = x.ApplicationForm.RequestType == null ? null : new Domain.Entities.RequestType
                    {
                        Id = x.ApplicationForm.RequestType.Id,
                        Name = x.ApplicationForm.RequestType.Name,
                        NameE = x.ApplicationForm.RequestType.NameE,
                    },
                    LeaveRequest = x.ApplicationForm.Leave == null ? null : new CommonDataHistoryApproval
                    {
                        Id = x.ApplicationForm.Leave.Id,
                        Code = x.ApplicationForm.Leave.Code,
                        UserNameRequestor = x.ApplicationForm.Leave.UserNameRequestor,
                        UserNameCreated = x.ApplicationForm.Leave.CreatedBy,
                        DepartmentName = x.ApplicationForm.Leave.OrgUnit == null ? "" : x.ApplicationForm.Leave.OrgUnit.Name
                    },
                    MemoNotification = x.ApplicationForm.MemoNotification == null ? null : new CommonDataHistoryApproval
                    {
                        Id = x.ApplicationForm.MemoNotification.Id,
                        Code = x.ApplicationForm.MemoNotification.Code,
                        UserNameRequestor = x.ApplicationForm.MemoNotification.CreatedBy,
                        UserNameCreated = x.ApplicationForm.MemoNotification.CreatedBy,
                        DepartmentName = x.ApplicationForm.MemoNotification.OrgUnit == null ? "" : x.ApplicationForm.MemoNotification.OrgUnit.Name,
                    },
                    ITForm = x.ApplicationForm.ITForm == null ? null : new CommonDataHistoryApproval
                    {
                        Id = x.ApplicationForm.ITForm.Id,
                        Code = x.ApplicationForm.ITForm.Code,
                        UserNameRequestor = x.ApplicationForm.ITForm.UserNameRequestor,
                        UserNameCreated = x.ApplicationForm.ITForm.UserNameCreated,
                        DepartmentName = x.ApplicationForm.ITForm.OrgUnit == null ? "" : x.ApplicationForm.ITForm.OrgUnit.Name,
                    }
                })
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

            var query = _context.ApplicationForms
                .Where(e =>
                    e.AssignedTasks.Any(at => at.UserCode == request.UserCode) &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                )
                .AsQueryable();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var results = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((int)(page - 1) * (int)pageSize)
                .Take((int)pageSize)
                .Select(r => new PendingApproval
                {
                    Id = r.Id,
                    RequestTypeId = r.RequestTypeId,
                    OrgPositionId = r.OrgPositionId,
                    CreatedAt = r.CreatedAt,
                    RequestType = r.RequestType,
                    HistoryApplicationForm = r.HistoryApplicationForms
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new HistoryApplicationForm { UserNameApproval = h.UserNameApproval })
                        .FirstOrDefault(),
                    LeaveRequest = r.Leave == null ? null : new CommonDataPendingApproval
                    {
                        Id = r.Leave.Id,
                        Code = r.Leave.Code,
                        UserNameRequestor = r.Leave.UserNameRequestor,
                        UserNameCreated = r.Leave.CreatedBy,
                        DepartmentName = r.Leave.OrgUnit == null ? "" : r.Leave.OrgUnit.Name
                    },
                    MemoNotification = r.MemoNotification == null ? null : new CommonDataPendingApproval
                    {
                        Id = r.MemoNotification.Id,
                        Code = r.MemoNotification.Code,
                        UserNameRequestor = r.MemoNotification.CreatedBy,
                        UserNameCreated = r.MemoNotification.CreatedBy,
                        DepartmentName = r.MemoNotification.OrgUnit == null ? "" : r.MemoNotification.OrgUnit.Name
                    },
                    ITForm = r.ITForm == null ? null : new CommonDataPendingApproval
                    {
                        Id = r.ITForm.Id,
                        Code = r.ITForm.Code,
                        UserNameRequestor = r.ITForm.UserNameRequestor,
                        UserNameCreated = r.ITForm.UserNameCreated,
                        DepartmentName = r.ITForm.OrgUnit == null ? "" : r.ITForm.OrgUnit.Name
                    }
                })
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
