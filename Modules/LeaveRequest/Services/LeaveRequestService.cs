using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Enums;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.DTO.Requests;
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
                        LastAction = actions.OrderByDescending(a => a.CreatedAt).FirstOrDefault()
                    }
                )
                .Select(x => new
                {
                    LeaveRequest = x.LeaveRequest,
                    LatestApprovalAction = x.LastAction
                });

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .Skip(((page - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();

            var tuples = pagedResult.Select(x => (x.LeaveRequest, x.LatestApprovalAction)).ToList();

            var dtos = LeaveRequestMapper.ToDtoList(tuples);

            return new PagedResults<LeaveRequestDto>
            {
                Data = dtos,
                TotalItems = totalItems,
                TotalPages = totalPages
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

            //get list user have position
            var userHaveNextPosition = await _context.Users
                .Where(e => e.PositionId == approvalFlow.ToPosition)
                .Select(x => new
                {
                    x.PositionId,
                    //email
                })
                .ToListAsync();

            //insert to table leave request
            var leaveRequest = LeaveRequestMapper.ToEntity(dto);

            _context.LeaveRequests.Add(leaveRequest);

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

            //1 -> meaning receive mail 
            if (userConfigReceiveEmail == null || (userConfigReceiveEmail != null && userConfigReceiveEmail.ConfigValue == "1"))
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

        public async Task<PagedResults<LeaveRequestDto>> GetAllWaitApproval(GetAllLeaveRequestWaitApprovalDto request)
        {
            int pageSize = request.PageSize;

            int page = request.Page;

            int? PositionId = request.PositionId;

            var user = await _context.Users
                .Include(ur => ur.UserRoles)
                    .ThenInclude(r => r.Role)
                    .FirstOrDefaultAsync(e => e.PositionId == PositionId);

            //check is hr or not

            var query1 = _context.LeaveRequests
        .Join(
            _context.ApprovalRequests,
            leave_rq => leave_rq.Id,
            approval_rq => approval_rq.RequestId,
            (leave_rq, approval_rq) => new { leave_rq, approval_rq }
        );

            // Áp dụng điều kiện lọc theo vai trò
            if (user.UserRoles.Any(e => e.Role != null &&
                (e.Role.Code == "hr" || e.Role.Code == "HR_Manager")))
            {
                query1 = query1.Where(x =>
                    x.approval_rq.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR &&
                    x.approval_rq.RequestType == "LEAVE_REQUEST");
            }
            else
            {
                query1 = query1.Where(x =>
                    x.approval_rq.CurrentPositionId == PositionId &&
                    (x.approval_rq.Status == StatusLeaveRequestEnum.IN_PROCESS.ToString() ||
                     x.approval_rq.Status == StatusLeaveRequestEnum.PENDING.ToString()) &&
                    x.approval_rq.RequestType == "LEAVE_REQUEST");
            }


            var finalQuery = query1
                .GroupJoin(
                    _context.ApprovalActions,
                    left => left.approval_rq.Id,
                    right => right.ApprovalRequestId,
                    (left, actions) => new
                    {
                        LeaveRequest = left.leave_rq,
                        LastAction = actions
                            .OrderByDescending(a => a.CreatedAt)
                            .FirstOrDefault()
                    }
                )
                .Select(x => new
                {
                    LeaveRequest = x.LeaveRequest,
                    LatestApprovalAction = x.LastAction
                });


            var query = _context.LeaveRequests
                .Join(
                    _context.ApprovalRequests,
                    leave_rq => leave_rq.Id,
                    approval_rq => approval_rq.RequestId,
                    (leave_rq, approval_rq) => new { leave_rq, approval_rq }
                    )
                .Where(x =>
                    x.approval_rq.CurrentPositionId == PositionId &&
                    (x.approval_rq.Status == StatusLeaveRequestEnum.IN_PROCESS.ToString() || x.approval_rq.Status == StatusLeaveRequestEnum.PENDING.ToString()) &&
                    x.approval_rq.RequestType == "LEAVE_REQUEST"
                )
                .GroupJoin(
                    _context.ApprovalActions,
                    left_approval_rq => left_approval_rq.approval_rq.Id,
                    approval_act => approval_act.ApprovalRequestId,
                    (left_approval_rq, actions) => new
                    {
                        LeaveRequest = left_approval_rq.leave_rq,
                        LastAction = actions.OrderByDescending(a => a.CreatedAt).FirstOrDefault()
                    }
                )
                .Select(x => new
                {
                    LeaveRequest = x.LeaveRequest,
                    LatestApprovalAction = x.LastAction
                });

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .Skip(((page - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();

            var tuples = pagedResult.Select(x => (x.LeaveRequest, x.LatestApprovalAction)).ToList();

            var dtos = LeaveRequestMapper.ToDtoList(tuples);

            return new PagedResults<LeaveRequestDto>
            {
                Data = dtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<int> CountWaitApproval(GetAllLeaveRequestWaitApprovalDto request)
        {
            var query = _context.LeaveRequests
                .Join(
                    _context.ApprovalRequests,
                    leave_rq => leave_rq.Id,
                    approval_rq => approval_rq.RequestId,
                    (leave_rq, approval_rq) => new { leave_rq, approval_rq }
                 )
                .Where(x =>
                    x.approval_rq.CurrentPositionId == request.PositionId &&
                    (x.approval_rq.Status == StatusLeaveRequestEnum.IN_PROCESS.ToString() || x.approval_rq.Status == StatusLeaveRequestEnum.PENDING.ToString()) &&
                    x.approval_rq.RequestType == "LEAVE_REQUEST")
                .Select(x => x.leave_rq);

            return await query.CountAsync();
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
            if (userApproval.UserRoles.Any(ur => ur.Role != null && ur.Role.Code == "hr") && approvalRequest.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR)
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
                //send email to hr
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));
            }
            else
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));
            }

            return null;

            //dont have any user approval
            //if (nextPosition == null)
            //{
            //    approvalRequest.Status = StatusLeaveRequestEnum.IN_PROCESS.ToString();
            //    approvalRequest.CurrentPositionId = (int?)StatusLeaveRequestEnum.WAIT_HR;
            //    _context.ApprovalRequests.Update(approvalRequest);

            //    var newApprovalAction = new Domain.Entities.ApprovalAction
            //    {
            //        ApprovalRequestId = approvalRequest.Id,
            //        ApproverUserCode = request.UserCodeApproval,
            //        ApproverName = request.NameUserApproval,
            //        Action = StatusLeaveRequestEnum.IN_PROCESS.ToString(),
            //        Comment = request.Note,
            //        CreatedAt = DateTimeOffset.Now
            //    };

            //    _context.ApprovalActions.Add(newApprovalAction);

            //    await _context.SaveChangesAsync();

            //    //send email to hr
            //    BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));

            //    return null;
            //}
            //else
            //{
            //    //update request current position to to_position_id
            //    approvalRequest.Status = StatusLeaveRequestEnum.IN_PROCESS.ToString();
            //    approvalRequest.CurrentPositionId = nextPosition.ToPosition;
            //    _context.ApprovalRequests.Update(approvalRequest);

            //    var newApprovalAction = new Domain.Entities.ApprovalAction
            //    {
            //        ApprovalRequestId = approvalRequest.Id,
            //        ApproverUserCode = request.UserCodeApproval,
            //        ApproverName = request.NameUserApproval,
            //        Action = StatusLeaveRequestEnum.IN_PROCESS.ToString(),
            //        Comment = request.Note,
            //        CreatedAt = DateTimeOffset.Now
            //    };

            //    _context.ApprovalActions.Add(newApprovalAction);

            //    await _context.SaveChangesAsync();

            //    //send email to hr
            //    BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));

            //    return null;

            //}


            //return null;

            //var userApproval = await _context.Users
            //    .Select(u => new
            //    {
            //        u.Id,
            //        u.Code,
            //        u.Name,
            //        u.Level,
            //        u.LevelParent,
            //        u.DepartmentId,
            //        u.Department,
            //        UserPermission = u.UserPermission
            //            .Where(up => up.Permission != null)
            //            .Select(up => up.Permission!.Name).ToList(),
            //        Roles = u.UserRoles
            //            .Where(ur => ur.Role != null)
            //            .Select(ur => ur.Role!.Code).ToList(),
            //        IsRoleHR = u.UserRoles
            //            .Any(ur => ur.Role != null && ur.Role.Code == "HR"),
            //        IsRoleHRManager = u.UserRoles
            //            .Any(ur => ur.Role != null && ur.Role.Code == "HR_Manager")
            //    })
            //    .FirstOrDefaultAsync(e => e.Code == request.UserCodeApproval);

            //if (userApproval == null)
            //{
            //    throw new NotFoundException("Not found user approval");
            //}

            ////check have custom 
            //var haveCustomApproval = await _context
            //    .CustomApprovalFlows
            //    .FirstOrDefaultAsync(e => e.TypeCustomApproval == "LEAVE_REQUEST" && e.DepartmentId == userApproval.DepartmentId && e.From == userApproval.Level);


            //Domain.Entities.User? nextUserCustomApproval = null;

            //bool flagHaveCustomApproval = false;

            //if (haveCustomApproval != null)
            //{
            //    nextUserCustomApproval = await _context.Users.Where(e => e.DepartmentId == userApproval.DepartmentId && e.Level == haveCustomApproval.To).FirstOrDefaultAsync();
            //    flagHaveCustomApproval = true;
            //}

            //var leaveRequest = await _context
            //    .LeaveRequests
            //    .FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? ""));

            //if (leaveRequest == null)
            //{
            //    throw new NotFoundException("Leave request not found!");
            //}

            //var leaveRequestStep = await _context.LeaveRequestSteps
            //    .FirstOrDefaultAsync(e =>
            //        e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING &&
            //        e.LeaveRequestId == Guid.Parse(request.LeaveRequestId ?? "")
            //    );

            //if (leaveRequestStep == null)
            //{
            //    throw new NotFoundException("Leave request step not found!");
            //}

            ////------- case reject

            //if (request.Status == false)
            //{
            //    leaveRequest.Status = (byte)StatusLeaveRequestEnum.REJECT;
            //    leaveRequest.Note = request.Note;
            //    _context.LeaveRequests.Update(leaveRequest);

            //    leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.REJECT;
            //    leaveRequestStep.ApprovedBy = userApproval.Name;
            //    leaveRequestStep.UserCodeApprover = userApproval.Code;
            //    leaveRequestStep.ApprovedAt = DateTime.Now;
            //    _context.LeaveRequestSteps.Update(leaveRequestStep);

            //    await _context.SaveChangesAsync();

            //    return LeaveRequestMapper.ToDto(leaveRequest);
            //}

            ////------- case approval

            ////case hr approval for other member
            //if ((userApproval.IsRoleHR && leaveRequestStep.LevelApproval == "HR") || userApproval.IsRoleHRManager)
            //{
            //    if (!userApproval.IsRoleHRManager)
            //    {
            //        if (!userApproval.Roles.Any(r => r != null && r.Contains("leave_request.hr_approval")))
            //        {
            //            throw new ForbiddenException("Bạn chưa có quyền, liên hệ team IT");
            //        }
            //    }

            //    leaveRequest.Status = (byte)StatusLeaveRequestEnum.COMPLETE;
            //    leaveRequest.UpdatedAt = DateTime.Now;
            //    _context.LeaveRequests.Update(leaveRequest);

            //    leaveRequestStep.UserCodeApprover = userApproval.Code;
            //    leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.APPROVAL;
            //    leaveRequestStep.ApprovedBy = userApproval.Name;
            //    leaveRequestStep.ApprovedAt = DateTime.Now;

            //    _context.LeaveRequestSteps.Update(leaveRequestStep);
            //    await _context.SaveChangesAsync();

            //    return LeaveRequestMapper.ToDto(leaveRequest);
            //}

            ////case hr approval for hr
            //if (userApproval.IsRoleHR)
            //{
            //    var nextUserApproval = await GetNextUserApproval(userApproval.DepartmentId, userApproval.LevelParent);

            //    leaveRequest.Status = nextUserApproval == null ? (byte)StatusLeaveRequestEnum.COMPLETE : (byte)StatusLeaveRequestEnum.IN_PROCESS;
            //    leaveRequest.UpdatedAt = DateTime.Now;
            //    _context.LeaveRequests.Update(leaveRequest);

            //    leaveRequestStep.UserCodeApprover = userApproval.Code;
            //    leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.APPROVAL;
            //    leaveRequestStep.ApprovedBy = userApproval.Name;
            //    leaveRequestStep.ApprovedAt = DateTime.Now;
            //    _context.LeaveRequestSteps.Update(leaveRequestStep);

            //    if (nextUserApproval != null)
            //    {
            //        Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
            //        {
            //            LeaveRequestId = leaveRequest.Id,
            //            StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
            //            LevelApproval = nextUserApproval?.Level ?? null
            //        };

            //        _context.LeaveRequestSteps.Add(newStep);

            //        if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
            //        {
            //            BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { nextUserApproval.Email }, leaveRequest, request.UrlFrontEnd));
            //        }
            //    }

            //    await _context.SaveChangesAsync();

            //    return LeaveRequestMapper.ToDto(leaveRequest);
            //}

            //if (userApproval.Roles.Any(role => role != null && role.Contains("leave_request.approval_to_hr")))
            //{
            //    Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
            //    {
            //        LeaveRequestId = leaveRequest.Id,
            //        LevelApproval = "HR",
            //        StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
            //    };
            //    _context.LeaveRequestSteps.Add(newStep);

            //    //send email to group hr, now fake 
            //    BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));
            //}
            //else
            //{
            //    leaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;

            //    var nextUserApproval = await _context.Users.FirstOrDefaultAsync(e =>
            //        e.DepartmentId == userApproval.DepartmentId &&
            //        e.Level == userApproval.LevelParent
            //    );

            //    Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
            //    {
            //        LeaveRequestId = leaveRequest.Id,
            //        StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
            //    };

            //    newStep.LevelApproval = "HR";
            //    var email = "nguyenviet@vsvn.com.vn"; //email hr

            //    if ((nextUserApproval != null && flagHaveCustomApproval == false) || (flagHaveCustomApproval && nextUserCustomApproval == null))
            //    {
            //        newStep.LevelApproval = nextUserApproval?.Level;
            //        email = nextUserApproval?.Email;
            //    }
            //    else if (flagHaveCustomApproval && nextUserCustomApproval != null)
            //    {
            //        newStep.LevelApproval = nextUserCustomApproval.Level;
            //        email = nextUserCustomApproval.Email;
            //    }

            //    _context.LeaveRequestSteps.Add(newStep);

            //    if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
            //    {
            //        BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { email ?? "" }, leaveRequest, request.UrlFrontEnd));
            //    }
            //}

            //leaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;
            //leaveRequest.UpdatedAt = DateTime.Now;
            //_context.LeaveRequests.Update(leaveRequest);

            //leaveRequestStep.UserCodeApprover = userApproval.Code;
            //leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.APPROVAL;
            //leaveRequestStep.ApprovedBy = userApproval.Name;
            //leaveRequestStep.ApprovedAt = DateTime.Now;

            //_context.LeaveRequestSteps.Update(leaveRequestStep);
            //await _context.SaveChangesAsync();

            //return LeaveRequestMapper.ToDto(leaveRequest);
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

            //get user and email of user request
            //var userRequest = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == approvalRequest.RequesterUserCode);
            BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus("nguyenviet@vsvn.com.vn", leaveRequest, request.UrlFrontEnd, request.Note, false));
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

            //get user and email of user request
            //var userRequest = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == approvalRequest.RequesterUserCode);
            BackgroundJob.Enqueue<EmailService>(job => job.SendEmaiLeaveRequestMySelfStatus("nguyenviet@vsvn.com.vn", leaveRequest, request.UrlFrontEnd, request.Note, true));
        }


        //public IQueryable<Domain.Entities.LeaveRequest> QueryWaitApprovalHR(int? departmentId, string? level)
        //{
        //    var query = _context.LeaveRequests
        //        .Where(o => o.LeaveRequestSteps != null && 
        //            (o.LeaveRequestSteps.Any(o => o.LevelApproval == "HR" && 
        //             o.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING) ||
        //        ((o.DepartmentId == departmentId) && 
        //            o.LeaveRequestSteps.Any(e => e.LevelApproval == level && e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING))));

        //    return query;
        //}

        //public IQueryable<Domain.Entities.LeaveRequest> QueryWaitApprovalNormalUser(int? departmentId, string? level)
        //{
        //    var query = _context.LeaveRequests.Where(e => 
        //            e.DepartmentId == departmentId && 
        //            e.LeaveRequestSteps != null &&
        //            e.LeaveRequestSteps.Any(x =>
        //                x.LevelApproval == level &&
        //                x.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING
        //            )
        //        );

        //    return query;
        //}

        //public async Task<Domain.Entities.User?> GetNextUserApproval(int? departmentId, string? level)
        //{
        //    var nextUserApproval = await _context.Users
        //        .FirstOrDefaultAsync(e =>
        //            e.DepartmentId == departmentId &&
        //            e.Level == level
        //        );

        //    return nextUserApproval;
        //}
    }
}
