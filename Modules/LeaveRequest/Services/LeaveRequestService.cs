using System.Security.Claims;
using Azure.Core;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Entities;
using ServicePortal.Domain.Enums;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.DTO.Requests;
using ServicePortal.Modules.LeaveRequest.DTO.Responses;
using ServicePortal.Modules.LeaveRequest.Services.Interfaces;

namespace ServicePortal.Modules.LeaveRequest.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<LeaveRequestDto>> GetAll(GetAllLeaveRequestDto request)
        {
            int pageSize = request.PageSize;

            int page = request.Page;

            string? status = request.Status;

            string? UserCode = request?.UserCode;

            var query = _context.LeaveRequests
                .Join(
                    _context.ApprovalRequests,
                    leave_rq => leave_rq.Id,
                    approval_rq => approval_rq.RequestId,
                    (leave_rq, approval_rq) => new { leave_rq, approval_rq }
                 )
                .Where(x =>
                    x.approval_rq.RequesterUserCode == UserCode &&
                    x.approval_rq.Status == status &&
                    x.approval_rq.RequestType == "LEAVE_REQUEST"
                )
                .GroupJoin(
                    _context.ApprovalActions,
                    left_approval_rq => left_approval_rq.approval_rq.Id,
                    approval_act => approval_act.ApprovalRequestId,
                    (left_approval_rq, actions) => new
                    {
                        LeaveRequest = left_approval_rq.leave_rq,
                        ApprovalRq = left_approval_rq.approval_rq,
                        LastAction = actions.OrderByDescending(a => a.CreatedAt).FirstOrDefault()
                    }
                )
                .Select(x => new
                {
                    LeaveRequest = x.LeaveRequest,
                    ApprovalRq = x.ApprovalRq,
                    LatestApprovalAction = x.LastAction
                });

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .Skip(((page - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();

            var tuples = pagedResult.Select(x => (x.LeaveRequest, x.ApprovalRq, x.LatestApprovalAction)).ToList();

            var dtos = LeaveRequestMapper.ToDtoList(tuples);

            var countPending = await _context
                .ApprovalRequests
                .Where(e =>
                    e.RequesterUserCode == UserCode &&
                    e.RequestType == "LEAVE_REQUEST" && 
                    e.Status == "PENDING"
                )
                .CountAsync();

            var countInProcess = await _context
                .ApprovalRequests
                .Where(e => 
                    e.RequesterUserCode == UserCode &&
                    e.RequestType == "LEAVE_REQUEST" && 
                    e.Status == "IN_PROCESS"
                )
                .CountAsync();

            return new PagedResults<LeaveRequestDto>
            {
                Data = dtos,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CountPending = countPending,
                CountInProcess = countInProcess,
            };
        }

        public async Task<LeaveRequestDto> GetById(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDto> Create(LeaveRequestDto dto)
        {
            //get user request
            var userRequester = await _context.Users
                .Where(e => e.UserCode == dto.RequesterUserCode)
                .Select(e => new
                {
                    e.UserCode,
                    e.PositionId
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found!");

            //get flow
            var approvalFlow = await _context.ApprovalFlows
                .Where(e => e.FromPosition == userRequester.PositionId)
                .Select(x => new
                {
                    x.ToPosition
                })
                .FirstOrDefaultAsync();

            //insert to table leave request
            var leaveRequest = LeaveRequestMapper.ToEntity(dto);
            _context.LeaveRequests.Add(leaveRequest);


            //if null -> default send to hr
            if (approvalFlow == null)
            {
                _context.ApprovalRequests.Add(new Domain.Entities.ApprovalRequest
                {
                    RequesterUserCode = dto.RequesterUserCode,
                    RequestType = "LEAVE_REQUEST",
                    RequestId = leaveRequest.Id,
                    Status = "IN_PROCESS",
                    CurrentPositionId = (int)StatusLeaveRequestEnum.WAIT_HR,
                    CreatedAt = DateTimeOffset.Now
                });

                await _context.SaveChangesAsync();

                return dto;
            }

            //get list user have position
            var userHaveNextPosition = await _context.Users
                .Where(e => e.PositionId == approvalFlow.ToPosition)
                .Select(x => new
                {
                    x.PositionId,
                    //email
                })
                .ToListAsync();

            //insert to table approval requet
            var approvalRequest = new Domain.Entities.ApprovalRequest
            {
                RequesterUserCode = dto.RequesterUserCode,
                RequestType = "LEAVE_REQUEST",
                RequestId = leaveRequest.Id,
                Status = "PENDING",
                CurrentPositionId = approvalFlow.ToPosition,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApprovalRequests.Add(approvalRequest);

            await _context.SaveChangesAsync();

            //get user config receive mail
            var userConfigReceiveEmail = await _context.UserConfigs
                .FirstOrDefaultAsync(e => e.UserCode == userRequester.UserCode && e.ConfigKey == "RECEIVE_MAIL_LEAVE_REQUEST");

            //true -> meaning receive mail 
            if (userConfigReceiveEmail == null || (userConfigReceiveEmail != null && userConfigReceiveEmail.ConfigValue == "true"))
            {
                //after get data email from viclock, now fake
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelf("nguyenviet@vsvn.com.vn", leaveRequest, dto.UrlFrontend));
            }

            //send email list same position
            foreach (var item in userHaveNextPosition)
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, dto.UrlFrontend));
            }

            return dto;
        }

        //fix
        public async Task<LeaveRequestDto> Update(Guid id, LeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            var updateEntity = LeaveRequestMapper.ToEntity(dto);

            _context.LeaveRequests.Update(updateEntity);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        //fix
        public async Task<LeaveRequestDto> Delete(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            _context.LeaveRequests.Remove(leaveRequest);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<PagedResults<LeaveRequestDto>> GetAllWaitApproval(GetAllLeaveRequestWaitApprovalDto request, ClaimsPrincipal userClaim)
        {
            int pageSize = request.PageSize;

            int page = request.Page;

            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var baseQuery = GetBaseLeaveRequestApprovalQuery(request, roleClaims);

            var finalQuery = baseQuery
                .GroupJoin(
                    _context.ApprovalActions,
                    left => left.ApprovalRequest != null ? left.ApprovalRequest.Id : Guid.NewGuid(),
                    right => right.ApprovalRequestId,
                    (left, actions) => new
                    {
                        LeaveRequest = left.LeaveRequest,
                        ApprovalRq = left.ApprovalRequest,
                        LastAction = actions
                            .OrderByDescending(a => a.CreatedAt)
                            .FirstOrDefault()
                    }
                )
                .Select(x => new
                {
                    LeaveRequest = x.LeaveRequest,
                    ApprovalRq = x.ApprovalRq,
                    LatestApprovalAction = x.LastAction
                });

            var totalItems = await finalQuery.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await finalQuery
                .Skip(((page - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();

            var tuples = pagedResult != null ? pagedResult.Select(x => (x.LeaveRequest, x.ApprovalRq, x.LatestApprovalAction)).ToList() : [];

            var dtos = LeaveRequestMapper.ToDtoList(tuples);

            return new PagedResults<LeaveRequestDto>
            {
                Data = dtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<int> CountWaitApproval(GetAllLeaveRequestWaitApprovalDto request, ClaimsPrincipal userClaim)
        {
            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var baseQuery = GetBaseLeaveRequestApprovalQuery(request, roleClaims);

            return await baseQuery.CountAsync();
        }

        public async Task<LeaveRequestDto?> Approval(ApprovalDto request, string currentUserCodeInJwt)
        {
            if (string.IsNullOrWhiteSpace(currentUserCodeInJwt))
            {
                throw new ForbiddenException("User forbidden!");
            }

            if (currentUserCodeInJwt.Trim() != request.UserCodeApproval)
            {
                throw new ForbiddenException("User forbidden!");
            }

            var userApproval = await _context.Users
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(e => e.UserCode == request.UserCodeApproval)
                ?? throw new NotFoundException("User not found!");

            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? ""))
                ?? throw new NotFoundException("User not found!");

            //case reject
            if (request.Status == false)
            {
                await RejectLeaveRq(request, leaveRequest);
                return null;
            }

            var approvalRequest = await _context.ApprovalRequests.FirstOrDefaultAsync(e => e.RequestId == leaveRequest.Id) ?? throw new NotFoundException("Not found data!");

            //case hr confirm register leave request
            if ((userApproval.UserRoles.Any(ur => ur.Role != null && ur.Role.Code == "HR") && approvalRequest.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR)
                || userApproval.UserRoles.Any(ur => ur.Role != null && ur.Role.Code == "HR_Manager"))
            {
                await HrApproval(request, leaveRequest);

                return null;
            }

            //check have next position approval in flow
            var nextPosition = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.FromPosition == userApproval.PositionId);

            bool isSendHr = false;

            if (nextPosition == null || userApproval.UserRoles.Any(ur => ur.Role != null && ur.Role.Code == "leave_request.approval_to_hr"))
            {
                approvalRequest.CurrentPositionId = (int?)StatusLeaveRequestEnum.WAIT_HR;
                isSendHr = true;
            }
            else
            {
                approvalRequest.CurrentPositionId = nextPosition.ToPosition;
            }

            approvalRequest.Status = StatusLeaveRequestEnum.IN_PROCESS.ToString();
            _context.ApprovalRequests.Update(approvalRequest);

            var newApprovalAction = new Domain.Entities.ApprovalAction
            {
                ApprovalRequestId = approvalRequest.Id,
                ApproverUserCode = request.UserCodeApproval,
                ApproverName = request.NameUserApproval,
                Action = StatusLeaveRequestEnum.IN_PROCESS.ToString(),
                Comment = request.Note,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApprovalActions.Add(newApprovalAction);

            await _context.SaveChangesAsync();

            if (isSendHr)
            {
                //send email to hr, now fake
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));
            }
            else
            {
                //send for next user position approval, now fake
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));
            }

            return null;
        }

        public async Task RejectLeaveRq(ApprovalDto request, Domain.Entities.LeaveRequest leaveRequest)
        {
            var approvalRequest = await _context.ApprovalRequests
                    .FirstOrDefaultAsync(e => e.RequestId == leaveRequest.Id)
                    ?? throw new NotFoundException("Not found data!");

            approvalRequest.Status = StatusLeaveRequestEnum.REJECT.ToString();
            _context.ApprovalRequests.Update(approvalRequest);

            var newApprovalAction = new Domain.Entities.ApprovalAction
            {
                ApprovalRequestId = approvalRequest.Id,
                ApproverUserCode = request.UserCodeApproval,
                ApproverName = request.NameUserApproval,
                Action = StatusLeaveRequestEnum.REJECT.ToString(),
                Comment = request.Note,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApprovalActions.Add(newApprovalAction);

            await _context.SaveChangesAsync();

            //get user config receive mail
            var userConfigReceiveEmail = await _context.UserConfigs
                .FirstOrDefaultAsync(e => e.UserCode == leaveRequest.RequesterUserCode && e.ConfigKey == "RECEIVE_MAIL_LEAVE_REQUEST");

            //true -> meaning receive mail 
            if (userConfigReceiveEmail == null || (userConfigReceiveEmail != null && userConfigReceiveEmail.ConfigValue == "true"))
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus("nguyenviet@vsvn.com.vn", leaveRequest, request.UrlFrontEnd, request.Note, false));
            }
        }

        public async Task HrApproval(ApprovalDto request, Domain.Entities.LeaveRequest leaveRequest)
        {
            var approvalRequest = await _context.ApprovalRequests
                    .FirstOrDefaultAsync(e => e.RequestId == leaveRequest.Id)
                    ?? throw new NotFoundException("Not found data!");

            approvalRequest.Status = StatusLeaveRequestEnum.COMPLETED.ToString();
            _context.ApprovalRequests.Update(approvalRequest);

            var newApprovalAction = new Domain.Entities.ApprovalAction
            {
                ApprovalRequestId = approvalRequest.Id,
                ApproverUserCode = request.UserCodeApproval,
                ApproverName = request.NameUserApproval,
                Action = StatusLeaveRequestEnum.COMPLETED.ToString(),
                Comment = request.Note,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApprovalActions.Add(newApprovalAction);

            await _context.SaveChangesAsync();

            //get user config receive mail
            var userConfigReceiveEmail = await _context.UserConfigs
                .FirstOrDefaultAsync(e => e.UserCode == leaveRequest.RequesterUserCode && e.ConfigKey == "RECEIVE_MAIL_LEAVE_REQUEST");

            //true -> meaning receive mail 
            if (userConfigReceiveEmail == null || (userConfigReceiveEmail != null && userConfigReceiveEmail.ConfigValue == "true"))
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus("nguyenviet@vsvn.com.vn", leaveRequest, request.UrlFrontEnd, request.Note, true));
            }
        }

        public IQueryable<LeaveRequestWithApprovalResponse> GetBaseLeaveRequestApprovalQuery(GetAllLeaveRequestWaitApprovalDto request, HashSet<string> roleClaims)
        {
            int? PositionId = request.PositionId;

            var query = _context.LeaveRequests
                .Join(
                    _context.ApprovalRequests,
                    leave_rq => leave_rq.Id,
                    approval_rq => approval_rq.RequestId,
                    (leave_rq, approval_rq) => new { leave_rq, approval_rq }
                );

            var statusList = new[] {
                StatusLeaveRequestEnum.IN_PROCESS.ToString(),
                StatusLeaveRequestEnum.PENDING.ToString()
            };

            if (roleClaims.Contains("HR") || roleClaims.Contains("HR_Manager"))
            {
                query = query.Where(x =>
                    x.approval_rq.RequestType == "LEAVE_REQUEST" &&
                    (
                        x.approval_rq.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR &&
                        (
                            statusList.Contains(x.approval_rq.Status)
                        )
                    ) ||
                    (
                        x.approval_rq.CurrentPositionId == PositionId &&
                        (
                            statusList.Contains(x.approval_rq.Status)
                        )
                    )
                );
            }
            else
            {
                query = query.Where(x =>
                    x.approval_rq.RequestType == "LEAVE_REQUEST" &&
                    (
                        x.approval_rq.CurrentPositionId == PositionId &&
                        (
                            statusList.Contains(x.approval_rq.Status)
                        )
                    )
                );
            }

            return query.Select(x => new LeaveRequestWithApprovalResponse
            {
                LeaveRequest = x.leave_rq,
                ApprovalRequest = x.approval_rq
            });
        }

        public async Task<string> HrRegisterAllLeave(HrRegisterAllLeaveRqDto request)
        {
            var statusList = new[] {
                StatusLeaveRequestEnum.IN_PROCESS.ToString(),
                StatusLeaveRequestEnum.PENDING.ToString()
            };

            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found");

            var result = await _context.ApprovalRequests
                .Where(e =>
                    e.RequestType == "LEAVE_REQUEST" &&
                    statusList.Contains(e.Status) && e.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR
                )
                .Join(_context.LeaveRequests, approvalRequest => approvalRequest.RequestId, leave => leave.Id, (approvalRequest, leave) => new
                {
                    ApprovalRequest = approvalRequest,
                    LeaveRequest = leave
                })
                .ToListAsync();

            List<ApprovalAction> actions = [];

            foreach (var item in result)
            {
                item.ApprovalRequest.Status = StatusLeaveRequestEnum.COMPLETED.ToString();
               
                actions.Add(new ApprovalAction
                {
                    ApprovalRequestId = item.ApprovalRequest.Id,
                    ApproverUserCode = user?.UserCode,
                    ApproverName = user?.UserCode?.ToString(),
                    Action = StatusLeaveRequestEnum.COMPLETED.ToString(),
                    CreatedAt = DateTimeOffset.Now
                });

                _context.ApprovalRequests.Update(item.ApprovalRequest);

                BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus("nguyenviet@vsvn.com.vn", item.LeaveRequest, request.UrlFrontEnd, null, true));
            }

            _context.ApprovalActions.AddRange(actions);

            await _context.SaveChangesAsync();

            return "success" ?? "error";
        }
    }
}
