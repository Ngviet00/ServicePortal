using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Interfaces.Approval;
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
        private readonly IMemoNotificationService _memoNotificationService;
        
        public ApprovalService
        (
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILeaveRequestService leaveRequestService,
            IMemoNotificationService memoNotificationService
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _leaveRequestService = leaveRequestService;
            _memoNotificationService = memoNotificationService;
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
                .AsQueryable();

            if (request.RequestTypeId != null)
            {
                query = query.Where(e => e.RequestTypeId == request.RequestTypeId);
            }

            if (isHR)
            {
                query = query.Where(e => e.CurrentOrgUnitId == request.OrgUnitId &&
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
                    e.CurrentOrgUnitId == request.OrgUnitId &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                );
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var results = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .Select(r => new PendingApproval
                {
                    Id = r.Id,
                    RequestTypeId = r.RequestTypeId,
                    CurrentOrgUnitId = r.CurrentOrgUnitId,
                    CreatedAt = r.CreatedAt,

                    RequestType = _context.RequestTypes
                        .Where(rt => rt.Id == r.RequestTypeId)
                        .FirstOrDefault(),

                    HistoryApplicationForm = r.HistoryApplicationForms
                        .OrderByDescending(h => h.CreatedAt)
                        .Select(h => new HistoryApplicationForm
                        {
                            UserApproval = h.UserApproval
                        })
                        .FirstOrDefault(),
                    LeaveRequest = _context.LeaveRequests
                        .Where(l => l.ApplicationFormId == r.Id)
                        .Select(l => new Domain.Entities.LeaveRequest
                        {
                            Id = l.Id,
                            Code = l.Code,
                            Name = l.Name,
                            UserNameWriteLeaveRequest = l.UserNameWriteLeaveRequest
                        })
                        .FirstOrDefault(),

                    MemoNotification = _context.MemoNotifications
                        .Where(mn => mn.ApplicationFormId == r.Id)
                        .Select(mn => new Domain.Entities.MemoNotification
                        {
                            Id =  mn.Id,
                            Code = mn.Code,
                            CreatedBy = mn.CreatedBy
                        })
                        .FirstOrDefault()

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

            return true;
        }

        public async Task<CountWaitApprovalAndAssignedInSidebarResponse> CountWaitAprrovalAndAssignedInSidebar(CountWaitAprrovalAndAssignedInSidebarRequest request)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var permissionClaims = userClaims?.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            bool isHR = roleClaims?.Contains("HR") == true && permissionClaims?.Contains("leave_request.hr_management_leave_request") == true;

            var query = _context.ApplicationForms
                .OrderByDescending(e => e.CreatedAt)
                .AsQueryable();

            if (isHR)
            {
                query = query.Where(e => e.CurrentOrgUnitId == request.OrgUnitId &&
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
                    e.CurrentOrgUnitId == request.OrgUnitId &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.COMPLETE &&
                    e.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                );
            }

            var totalItems = await query.CountAsync();

            CountWaitApprovalAndAssignedInSidebarResponse results = new CountWaitApprovalAndAssignedInSidebarResponse();

            results.TotalWaitApproval = totalItems;
            results.TotalAssigned = 0;

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

            var query = _context.HistoryApplicationForms.Where(e => e.UserCodeApproval == userCode);

            if (requestTypeId != null)
            {
                query = query.Where(e => e.ApplicationForm != null && e.ApplicationForm.RequestTypeId == requestTypeId);
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var results = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .Select(x => new HistoryApprovalProcessResponse
                {
                    CreatedAt = x.CreatedAt,
                    ActionType  = x.ActionType,
                    RequestStatusId = x.ApplicationForm != null ? x.ApplicationForm.RequestStatusId : null,
                    RequestType = x.ApplicationForm != null && x.ApplicationForm.RequestType == null ? null : new Domain.Entities.RequestType
                    {
                        Id = x.ApplicationForm.RequestType.Id,
                        Name = x.ApplicationForm.RequestType.Name,
                        NameE = x.ApplicationForm.RequestType.NameE,
                    },
                    LeaveRequest = x.ApplicationForm.Leave == null ? null : new Domain.Entities.LeaveRequest
                    {
                        Id = x.ApplicationForm.Leave.Id,
                        Code = x.ApplicationForm.Leave.Code,
                        Name = x.ApplicationForm.Leave.Name
                    },
                    MemoNotification = x.ApplicationForm.MemoNotification == null ? null : new Domain.Entities.MemoNotification
                    {
                        Id = x.ApplicationForm.MemoNotification.Id,
                        Code = x.ApplicationForm.MemoNotification.Code,
                        CreatedBy = x.ApplicationForm.MemoNotification.CreatedBy
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

        public Task<object> ListAssigned()
        {
            throw new NotImplementedException();
        }
    }
}
