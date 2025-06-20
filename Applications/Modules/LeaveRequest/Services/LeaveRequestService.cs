using System.Security.Claims;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.HRManagement.Services;
using ServicePortal.Applications.Modules.LeaveRequest.DTO;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Responses;
using ServicePortal.Applications.Modules.LeaveRequest.Services.Interfaces;
using ServicePortal.Applications.Modules.User.Services.Interfaces;
using ServicePortal.Common;
using ServicePortal.Common.Helpers;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Entities;
using ServicePortal.Domain.Enums;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;

namespace ServicePortal.Applications.Modules.LeaveRequest.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IHRManagementService _hrManagementService;

        public LeaveRequestService(ApplicationDbContext context, IUserService userService, IHRManagementService hrManagementService)
        {
            _context = context;
            _userService = userService;
            _hrManagementService = hrManagementService;
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
                    (x.approval_rq.RequesterUserCode == UserCode || x.leave_rq.WriteLeaveUserCode == UserCode) &&
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
                    x.LeaveRequest,
                    x.ApprovalRq,
                    LatestApprovalAction = x.LastAction
                });

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.LeaveRequest.CreatedAt)
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
            //lấy thông tin người viết đơn
            var userRequester = await _context.Users
                .Where(e => e.UserCode == dto.RequesterUserCode)
                .Select(e => new
                {
                    e.UserCode,
                    e.PositionId,
                    e.Email
                })
                .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found!");

            //luồng xin nghỉ phép
            var approvalFlow = await _context.ApprovalFlows
                .Where(e => e.FromPosition == userRequester.PositionId)
                .Select(x => new
                {
                    x.ToPosition
                })
                .FirstOrDefaultAsync();

            //lưu vào db bảng leave_requests
            var leaveRequest = LeaveRequestMapper.ToEntity(dto);
            _context.LeaveRequests.Add(leaveRequest);


            //nếu như luồng k còn ai duyệt tiếp, gửi đến HR
            if (approvalFlow == null)
            {
                //lưu vào bảng approval_requests, với current_position_id là -10 (HR)
                _context.ApprovalRequests.Add(new ApprovalRequest
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

            //lấy thông tin người duyệt tiếp theo
            var userHaveNextPosition = await _context.Users
                .Where(e => e.PositionId == approvalFlow.ToPosition)
                .Select(x => new
                {
                    x.PositionId,
                    x.Email
                })
                .ToListAsync();

            //lưu vào bảng approval_requests, với current_position_id là position tiếp theo
            var approvalRequest = new ApprovalRequest
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

            //gửi email cho người xin nghỉ, check kiểm tra người dùng có config nhận thông báo email không
            //nếu như check là k nhận thông báo thì k gửi, ngược lại các trường hợp sẽ gửi
            var userConfigReceiveEmail = await _context.UserConfigs.FirstOrDefaultAsync(e =>
                e.UserCode == userRequester.UserCode && 
                e.ConfigKey == "RECEIVE_MAIL_LEAVE_REQUEST"
            );
            if (userConfigReceiveEmail == null || userConfigReceiveEmail != null && userConfigReceiveEmail.ConfigValue == "true")
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelf(!string.IsNullOrWhiteSpace(userRequester.Email) ? userRequester.Email : Global.EmailDefault, leaveRequest, dto.UrlFrontend));
            }

            //gửi email cho người duyệt tiếp theo
            var toEmails = new List<string>();
            foreach (var item in userHaveNextPosition)
            {
                toEmails.Add(!string.IsNullOrWhiteSpace(item.Email) ? item.Email : Global.EmailDefault);
            }
            BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(toEmails, new List<string>() { }, new List<Domain.Entities.LeaveRequest> { leaveRequest }, dto.UrlFrontend));

            return dto;
        }

        public async Task<LeaveRequestDto> Update(Guid id, LeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            leaveRequest.RequesterUserCode = dto.RequesterUserCode;
            leaveRequest.Name = dto.Name;
            leaveRequest.Department = dto.Department;
            leaveRequest.Position = dto.Position;

            leaveRequest.FromDate = DateTimeOffset.Parse(dto.FromDate ?? "");
            leaveRequest.ToDate = DateTimeOffset.Parse(dto.ToDate ?? "");

            leaveRequest.TypeLeave = dto.TypeLeave;
            leaveRequest.TimeLeave = dto.TimeLeave;
            leaveRequest.Reason = dto.Reason;

            _context.LeaveRequests.Update(leaveRequest);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDto> Delete(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            _context.LeaveRequests.Remove(leaveRequest);

            var requestOfLeave = await _context.ApprovalRequests.FirstOrDefaultAsync(e => e.RequestId == id) ?? throw new NotFoundException("Leave request not found!");

            _context.ApprovalRequests.Remove(requestOfLeave);

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
                        left.LeaveRequest,
                        ApprovalRq = left.ApprovalRequest,
                        LastAction = actions
                            .OrderByDescending(a => a.CreatedAt)
                            .FirstOrDefault()
                    }
                )
                .Select(x => new
                {
                    x.LeaveRequest,
                    x.ApprovalRq,
                    LatestApprovalAction = x.LastAction
                });

            var totalItems = await finalQuery.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await finalQuery
                .Skip((page - 1) * pageSize)
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
                ?? throw new NotFoundException("Leave request not found!");

            //case reject
            if (request.Status == false)
            {
                await RejectLeaveRq(request, leaveRequest);
                return null;
            }

            var approvalRequest = await _context.ApprovalRequests.FirstOrDefaultAsync(e => e.RequestId == leaveRequest.Id) ?? throw new NotFoundException("Not found data!");

            //case hr confirm register leave request
            if (userApproval.UserRoles.Any(ur => ur.Role != null && ur.Role.Code == "HR") && approvalRequest.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR
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

            var newApprovalAction = new ApprovalAction
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
                var emailHR = await _hrManagementService.GetEmailHRByType("MANAGE_TIMEKEEPING");
                var listEmailHRs = new List<string>();

                foreach (var item in emailHR)
                {
                    listEmailHRs.Add(!string.IsNullOrWhiteSpace(item) ? item : Global.EmailDefault);
                }

               BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(listEmailHRs, new List<string> { userApproval.Email ?? Global.EmailDefault } , new List<Domain.Entities.LeaveRequest> { leaveRequest }, request.UrlFrontEnd));
            }
            else
            {
                var emailNextPosition = await _userService.GetUserByPosition(nextPosition?.ToPosition);

                var listToEmails = new List<string>();

                foreach (var item in emailNextPosition)
                {
                    string email = Global.EmailDefault;
                    if (item != null && !string.IsNullOrWhiteSpace(item.Email))
                    {
                        email = item.Email;
                    }
                    listToEmails.Add(email);
                }

                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(listToEmails, new List<string> { userApproval.Email ?? Global.EmailDefault } , new List<Domain.Entities.LeaveRequest> { leaveRequest }, request.UrlFrontEnd));
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

            var newApprovalAction = new ApprovalAction
            {
                ApprovalRequestId = approvalRequest.Id,
                ApproverUserCode = request.UserCodeApproval,
                ApproverName = request.NameUserApproval,
                Action = StatusLeaveRequestEnum.REJECT.ToString(),
                Comment = request.Note,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApprovalActions.Add(newApprovalAction);

            var checkEmail = await _userService.GetEmailByUserCodeAndUserConfig(new List<string> { leaveRequest?.RequesterUserCode ?? "" });

            var firstEmail = checkEmail.FirstOrDefault();

            var email = firstEmail != null && !string.IsNullOrWhiteSpace(firstEmail.Email) ? firstEmail.Email : Global.EmailDefault;

            BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus(email, leaveRequest, request.UrlFrontEnd, request.Note, false));

            await _context.SaveChangesAsync();
        }

        public async Task HrApproval(ApprovalDto request, Domain.Entities.LeaveRequest leaveRequest)
        {
            var approvalRequest = await _context.ApprovalRequests
                    .FirstOrDefaultAsync(e => e.RequestId == leaveRequest.Id)
                    ?? throw new NotFoundException("Not found data!");

            approvalRequest.Status = StatusLeaveRequestEnum.COMPLETED.ToString();
            _context.ApprovalRequests.Update(approvalRequest);

            var newApprovalAction = new ApprovalAction
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

            var checkEmail = await _userService.GetEmailByUserCodeAndUserConfig(new List<string> { leaveRequest?.RequesterUserCode ?? "" });

            var firstEmail = checkEmail.FirstOrDefault();

            var email = firstEmail != null && !string.IsNullOrWhiteSpace(firstEmail?.Email) ? firstEmail.Email : Global.EmailDefault;

            BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus(email, leaveRequest, request.UrlFrontEnd, request.Note, true));
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
                    
                        x.approval_rq.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR &&
                        
                            statusList.Contains(x.approval_rq.Status)
                        
                     ||
                    
                        x.approval_rq.CurrentPositionId == PositionId &&
                        
                            statusList.Contains(x.approval_rq.Status)
                        
                    
                );
            }
            else
            {
                query = query.Where(x =>
                    x.approval_rq.RequestType == "LEAVE_REQUEST" &&
                    
                        x.approval_rq.CurrentPositionId == PositionId &&
                        
                            statusList.Contains(x.approval_rq.Status)
                        
                    
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
                    ApproverUserCode = request?.UserCode,
                    ApproverName = request?.UserName,
                    Action = StatusLeaveRequestEnum.COMPLETED.ToString(),
                    CreatedAt = DateTimeOffset.Now
                });

                _context.ApprovalRequests.Update(item.ApprovalRequest);

                var checkEmail = await _userService.GetEmailByUserCodeAndUserConfig(new List<string> { item.ApprovalRequest.RequesterUserCode ?? Global.EmailDefault });
                var firstEmail = checkEmail.FirstOrDefault();
                var email = firstEmail != null && !string.IsNullOrWhiteSpace(firstEmail.Email) ? firstEmail.Email : Global.EmailDefault;

                BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus(email, item.LeaveRequest, null, null, true));
            }

            _context.ApprovalActions.AddRange(actions);

            await _context.SaveChangesAsync();

            return "success" ?? "error";
        }

        public async Task<PagedResults<HistoryLeaveRequestApprovalResponse>> GetHistoryLeaveRequestApproval(GetAllLeaveRequestDto request)
        {
            double pageSize = request.PageSize;

            double page = request.Page;

            string? UserCode = request?.UserCode;

            var query = _context.LeaveRequests
                .Join(_context.ApprovalRequests,
                    LR => LR.Id,
                    AR => AR.RequestId,
                    (LR, AR) => new { LR, AR }
                )
                .Join(_context.ApprovalActions,
                    LR_AR => LR_AR.AR.Id,
                    AA => AA.ApprovalRequestId,
                    (LR_AR, AA) => new { LR_AR.LR, LR_AR.AR, AA }
                )
                .Where(x =>
                    x.AR.Status == StatusLeaveRequestEnum.COMPLETED.ToString() &&
                    x.AR.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR &&
                    x.AA.ApproverUserCode == UserCode &&
                    x.AA.Action == StatusLeaveRequestEnum.COMPLETED.ToString()
                );

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var results = await query
                .OrderByDescending(x => x.AA.CreatedAt)
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .Select(x => new HistoryLeaveRequestApprovalResponse
                {
                    RequesterUserCode = x.LR.RequesterUserCode,
                    Name = x.LR.Name,
                    Department = x.LR.Department,
                    Position = x.LR.Position,
                    FromDate = x.LR.FromDate,
                    ToDate = x.LR.ToDate,
                    TypeLeave = x.LR.TypeLeave,
                    TimeLeave = x.LR.TimeLeave,
                    Reason = x.LR.Reason,
                    ApproverName = x.AA.ApproverName,
                    ApprovalAt = x.AA.CreatedAt
                })
                .ToListAsync();

            var data = new PagedResults<HistoryLeaveRequestApprovalResponse>
            {
                Data = results,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return data;
        }

        public async Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request)
        {
            if (request.Leaves != null && request.Leaves.Count > 0)
            {
                var userCodeWriteRequester = request.Leaves.First().WriteLeaveUserCode;

                //lấy thông tin người viết đơn cho người khác
                var userWriteRequester = await _context.Users
                    .Where(e => e.UserCode == userCodeWriteRequester)
                    .Select(e => new
                    {
                        e.UserCode,
                        e.PositionId,
                        e.Email
                    })
                    .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found!");

                //luồng duyệt
                var approvalFlow = await _context.ApprovalFlows
                    .Where(e => e.FromPosition == userWriteRequester.PositionId)
                    .Select(x => new
                    {
                        x.ToPosition
                    })
                    .FirstOrDefaultAsync();

                //nếu hết người duyệt, gửi đến HR
                if (approvalFlow == null)
                {
                    foreach (var itemLeave in request.Leaves)
                    {
                        //lưu bảng leave_requests và approval_requests
                        var newLeave = LeaveRequestMapper.ToEntity(itemLeave);
                        _context.LeaveRequests.Add(newLeave);

                        _context.ApprovalRequests.Add(new ApprovalRequest
                        {
                            RequesterUserCode = itemLeave.RequesterUserCode,
                            RequestType = "LEAVE_REQUEST",
                            RequestId = newLeave.Id,
                            Status = "IN_PROCESS",
                            CurrentPositionId = (int)StatusLeaveRequestEnum.WAIT_HR,
                            CreatedAt = DateTimeOffset.Now
                        });

                        await _context.SaveChangesAsync();
                    }

                    return true;
                }

                //lấy những người duyệt tiếp theo
                var userHaveNextPosition = await _context.Users
                    .Where(e => e.PositionId == approvalFlow.ToPosition)
                    .Select(x => new
                    {
                        x.PositionId,
                        x.Email
                    })
                    .ToListAsync();

                foreach (var itemLeave in request.Leaves)
                {
                    var newLeave = LeaveRequestMapper.ToEntity(itemLeave);
                    _context.LeaveRequests.Add(newLeave);

                    _context.ApprovalRequests.Add(new ApprovalRequest
                    {
                        RequesterUserCode = itemLeave.RequesterUserCode,
                        RequestType = "LEAVE_REQUEST",
                        RequestId = newLeave.Id,
                        Status = "PENDING",
                        CurrentPositionId = approvalFlow.ToPosition,
                        CreatedAt = DateTimeOffset.Now
                    });
                }

                //gửi email đến người duyệt tiếp theo
                var toEmails = new List<string>();
                foreach (var item in userHaveNextPosition)
                {
                    toEmails.Add(!string.IsNullOrWhiteSpace(item.Email) ? item.Email : Global.EmailDefault);
                }

                //gửi email cho người xin nghỉ, check kiểm tra người dùng có config nhận thông báo email không
                //nếu như check là k nhận thông báo thì k gửi, ngược lại các trường hợp sẽ gửi
                var ccEmails = new List<string>();
                var getEmailByUserCodeAndUserConfig = await _userService.GetEmailByUserCodeAndUserConfig([.. request.Leaves.Select(x => x.RequesterUserCode)]);
                
                foreach (var item in getEmailByUserCodeAndUserConfig)
                {
                    ccEmails.Add(!string.IsNullOrWhiteSpace(item.Email) ? item.Email : Global.EmailDefault);
                }

                ccEmails.Add(!string.IsNullOrWhiteSpace(userWriteRequester.Email) ? userWriteRequester.Email : Global.EmailDefault);

                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(toEmails, ccEmails, LeaveRequestMapper.ToEntityList(request.Leaves), request.Leaves[0].UrlFrontend));

                await _context.SaveChangesAsync();

                return true;
            }

            throw new Exception("Không có người nào xin nghỉ phép");
        }
    }
}
