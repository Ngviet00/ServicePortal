using System.Security.Claims;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Interfaces;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Application.Interfaces.WorkFlowStep;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.LeaveRequest
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IOrgUnitService _orgUnitService;
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly IWorkFlowStepService _workFlowStepService;
        private readonly ICommonDataService _commonDataService;
        private const int ORG_UNIT_ID_COMPLETE_DONE = -10;

        public LeaveRequestService(
            ApplicationDbContext context,
            IUserService userService,
            IOrgUnitService orgUnitService,
            IViclockDapperContext viclockDapperContext,
            IWorkFlowStepService workFlowStepService,
            ICommonDataService commonDataService
        )
        {
            _context = context;
            _userService = userService;
            _orgUnitService = orgUnitService;
            _viclockDapperContext = viclockDapperContext;
            _workFlowStepService = workFlowStepService;
            _commonDataService = commonDataService;
        }

        public async Task<PagedResults<LeaveRequestDto>> GetAll(GetAllLeaveRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;
            int? status = request.Status;
            string? UserCode = request?.UserCode;

            var query = _context.LeaveRequests
                .Include(l => l.TimeLeave)
                .Include(l => l.TypeLeave)
                .Include(l => l.ApplicationForm)
                    .ThenInclude(a => a.HistoryApplicationForms)
                .Where(l => l.RequesterUserCode == UserCode &&
                            l.ApplicationForm != null &&
                            (
                                status == 2
                                    ? (l.ApplicationForm.RequestStatusId == 2 || l.ApplicationForm.RequestStatusId == 4)
                                    : l.ApplicationForm.RequestStatusId == status
                            )
                );

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var dtos = LeaveRequestMapper.ToDtoList(pagedResult);

            var countPending = await _context.LeaveRequests
                .Include(e => e.ApplicationForm)
                .Where(e => e.RequesterUserCode == UserCode && e.ApplicationForm != null && e.ApplicationForm.RequestStatusId == 1) //1 pending
                .CountAsync();

            var countInProcess = await _context.LeaveRequests
                .Include(e => e.ApplicationForm)
                .Where(e => e.RequesterUserCode == UserCode && e.ApplicationForm != null && 
                    (
                        e.ApplicationForm.RequestStatusId == 2 || e.ApplicationForm.RequestStatusId == 4
                    )
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
            var leaveRequest = await _context.LeaveRequests
                .Include(e => e.TimeLeave)
                .Include(e => e.TypeLeave)
                .FirstOrDefaultAsync(e => e.Id == id) 
                ?? throw new NotFoundException("Leave request not found!");

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDto> Create(CreateLeaveRequest request)
        {
            //get current user info
            var requesterUser = await _context.Users
                .Include(e => e.UserConfigs)
                .FirstOrDefaultAsync(e => e.UserCode == request.RequesterUserCode);

            if (requesterUser == null)
            {
                throw new NotFoundException("User not found!");
            }

            dynamic? orgUnitIdCurrentUser = await _userService.GetCustomColumnUserViclockByUserCode(request.RequesterUserCode ?? "", "OrgUnitID");
            if (string.IsNullOrWhiteSpace(orgUnitIdCurrentUser?.OrgUnitID?.ToString()))
            {
                throw new ValidationException("Thông tin chưa được cập nhật, vui lòng liên hệ với HR!");
            }

            //get org unit id current user
            int.TryParse(orgUnitIdCurrentUser?.OrgUnitID?.ToString(), out int orgUnitId);
            var name = orgUnitIdCurrentUser?.NVHoTen;

            //lấy thông tin request type, 1 = nghỉ phép
            var requestType = await _context.RequestTypes.FirstOrDefaultAsync(e => e.Id == 1) ?? throw new ValidationException("Loại yêu cầu không hợp lệ!");

            var requestStatus = await _commonDataService.GetAllRequestStatus();
            var timeLeave = await _commonDataService.GetAllTimeLeave();

            var typeLeave = await _context.TypeLeaves.FirstOrDefaultAsync(e => e.Id == request.TypeLeaveId);

            //get orgUnitId next user
            var currentOrgUnit = await _orgUnitService.GetOrgUnitById(orgUnitId);
            var nextOrgUnitId = currentOrgUnit?.ParentJobTitleId;

            ApplicationForm newApplicationForm = new()
            {
                RequesterUserCode = request.RequesterUserCode,
                RequestTypeId = requestType.Id,
                CurrentOrgUnitId = nextOrgUnitId,
                CreatedAt = DateTimeOffset.Now
            };

            //check have custom in workflowstep, request = 1 nghi phep vaf specific bang send to hr
            WorkFlowStep getWorkFlowStepByRequestType = await _workFlowStepService.GetWorkFlowByFromOrgUnitIdAndRequestType(nextOrgUnitId, requestType.Id);

            bool isSendHr = false;
            if (nextOrgUnitId == null || (getWorkFlowStepByRequestType != null && getWorkFlowStepByRequestType.ToOrgUnitContext == "SPECIFIC_SEND_HR"))
            {
                newApplicationForm.RequestStatusId = requestStatus?.FirstOrDefault(e => e.Name == "WAIT_HR")?.Id; //WAIT_HR
                isSendHr = true;
            }
            else
            {
                newApplicationForm.RequestStatusId = requestStatus?.FirstOrDefault(e => e.Name == "PENDING")?.Id; //PENDING
            }

            _context.ApplicationForms.Add(newApplicationForm);
            Domain.Entities.LeaveRequest leaveRequest = new()
            {
                ApplicationFormId = newApplicationForm.Id,
                RequesterUserCode = request.RequesterUserCode,
                Name = request.Name,
                Department = request.Department,
                Position = request.Position,
                UserNameWriteLeaveRequest = request.UserNameWriteLeaveRequest,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TimeLeaveId = request.TimeLeaveId,
                TypeLeaveId = request.TypeLeaveId,
                Reason = request.Reason,
                CreatedAt = DateTimeOffset.Now
            };
           
            _context.LeaveRequests.Add(leaveRequest);

            //save change aysnc
            await _context.SaveChangesAsync();

            //check have setting not receive email
            if (!(requesterUser?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false"))
            {
                //send email for current user
                string bodyMailPersonal = TemplateEmail.EmailContentLeaveRequest(leaveRequest, typeLeave, timeLeave?.FirstOrDefault(e => e.Id == request.TimeLeaveId));
                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        new List<string> { requesterUser.Email ?? "" },
                        null,
                        "Đơn xin nghỉ phép đã được gửi",
                        bodyMailPersonal,
                        null,
                        true
                    )
                );
            }

            if (isSendHr)
            {
                //find hr and send to hr
            }
            else
            {
                //check more condition have cross management
                string sqlFindNextUser = $@"
                    SELECT 
                        u.UserCode,
                        u.Email,
                        nv.NVEmail,
                        nv.OrgUnitID
                    FROM 
                        tblNhanVien AS nv
                    LEFT JOIN ServicePortal.dbo.users AS u
                    ON nv.NVMaNV = u.UserCode
                    WHERE nv.OrgUnitID = @nextOrgUnitId
                ";

                var param = new
                {
                    nextOrgUnitId = nextOrgUnitId
                };

                dynamic? nextUserData = await _viclockDapperContext.QueryAsync<dynamic>(sqlFindNextUser, param);

                List<string> toEmails = [];

                foreach (var item in nextUserData)
                {
                    toEmails.Add(item.Email ?? item.NVEmail ?? "");
                }

                string urlWaitApproval = $"{request.UrlFrontend}/leave/wait-approval";
                string bodyMail = $@"
                    <h4>
                        <span>Duyệt đơn: </span>
                        <a href={urlWaitApproval}>{urlWaitApproval}</a>
                    </h4>" + TemplateEmail.EmailContentLeaveRequest(leaveRequest, typeLeave, timeLeave?.FirstOrDefault(e => e.Id == request.TimeLeaveId)) + "<br/>";

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        toEmails,
                        null,
                        $"Đơn xin nghỉ phép - {requesterUser.UserCode} - {name}",
                        bodyMail,
                        null,
                        true
                    )
                );
            }

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDto> Update(Guid id, LeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            leaveRequest.Id = leaveRequest.Id;
            leaveRequest.Department = dto.Department;
            leaveRequest.Position = dto.Position;

            leaveRequest.FromDate = dto.FromDate;
            leaveRequest.ToDate = dto.ToDate;

            leaveRequest.TimeLeaveId = dto.TimeLeaveId;
            leaveRequest.TypeLeaveId = dto.TypeLeaveId;
            leaveRequest.Reason = dto.Reason;
            leaveRequest.UpdateAt = DateTimeOffset.Now;

            _context.LeaveRequests.Update(leaveRequest);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDto> Delete(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            _context.LeaveRequests.Remove(leaveRequest);

            var requestOfLeave = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == leaveRequest.ApplicationFormId) ?? throw new NotFoundException("Leave request not found!");

            _context.ApplicationForms.Remove(requestOfLeave);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<PagedResults<LeaveRequestDto>> GetAllWaitApproval(GetAllLeaveRequestWaitApprovalRequest request, ClaimsPrincipal userClaim)
        {
            int pageSize = request.PageSize;
            int page = request.Page;

            var query = GetBaseLeaveRequestApprovalQuery(request, userClaim)
                .Include(e => e.TimeLeave)
                .Include(e => e.TypeLeave)
                .Include(e => e.ApplicationForm)
                    .ThenInclude(a => a.HistoryApplicationForms);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = LeaveRequestMapper.ToDtoList(pagedResult);

            return new PagedResults<LeaveRequestDto>
            {
                Data = dtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<int> CountWaitApproval(GetAllLeaveRequestWaitApprovalRequest request, ClaimsPrincipal userClaim)
        {
            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var baseQuery = GetBaseLeaveRequestApprovalQuery(request, userClaim);

            return await baseQuery.CountAsync();
        }

        public IQueryable<Domain.Entities.LeaveRequest> GetBaseLeaveRequestApprovalQuery(GetAllLeaveRequestWaitApprovalRequest request, ClaimsPrincipal userClaim)
        {
            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            int? orgUnitId = request.OrgUnitId;

            var q = _context.LeaveRequests.AsQueryable();

            if (roleClaims.Contains("HR"))
            {
                q = q.Where(e => 
                    (
                        e.ApplicationForm != null && e.ApplicationForm.CurrentOrgUnitId == orgUnitId &&
                        (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.PENDING || e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS)
                    ) ||
                    (e.ApplicationForm != null && e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR)
                );
            }
            else
            {
                q = q.Where(e =>
                    e.ApplicationForm != null &&
                    e.ApplicationForm.CurrentOrgUnitId == orgUnitId &&
                    (
                        e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.PENDING ||
                        e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS
                    )
                );
            }

            return q;
        }

        public async Task<LeaveRequestDto?> Approval(ApprovalRequests request, string currentUserCodeInJwt, ClaimsPrincipal userClaim)
        {
            if (string.IsNullOrWhiteSpace(currentUserCodeInJwt) || currentUserCodeInJwt.Trim() != request.UserCodeApproval)
            {
                throw new ForbiddenException("User forbidden!");
            }

            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            //lấy thông tin người approval ở viclock => lấy đc OrgUnitId, họ tên
            dynamic? userApprovalInViclock = await _userService.GetCustomColumnUserViclockByUserCode(
                currentUserCodeInJwt ?? "", "OrgUnitID, NVEmail"
            ) ?? throw new NotFoundException("User not found in Viclock");

            //lấy thông tin người approval ở web system => email, user config...
            var userApprovalInWebSystem = await _context.Users
                .Include(e => e.UserConfigs)
                .FirstOrDefaultAsync(e => e.UserCode == currentUserCodeInJwt)
                ?? throw new NotFoundException("User not found in Web system");

            var leaveRequest = await _context.LeaveRequests
                .Include(e => e.User)
                    .ThenInclude(e => e.UserConfigs)
                .Include(e => e.TimeLeave)
                .Include(e => e.TypeLeave)
                .Include(e => e.ApplicationForm)
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? ""))
                ?? throw new NotFoundException("Leave request not found");

            var userRequester = leaveRequest.User;

            var applicationForm = leaveRequest.ApplicationForm ?? throw new NotFoundException("Application Form not found");

            var orgUnit = await _orgUnitService.GetOrgUnitById(userApprovalInViclock?.OrgUnitID);

            var historyApplicationForm = new HistoryApplicationForm
            {
                ApplicationFormId = applicationForm.Id,
                UserApproval = userApprovalInViclock?.NVHoTen,
                CreatedAt = DateTimeOffset.Now
            };

            //reject
            if (request.Status == false)
            {
                applicationForm.Id = applicationForm.Id;
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;

                historyApplicationForm.UserApproval = request.NameUserApproval;
                historyApplicationForm.ActionType = "REJECT";
                historyApplicationForm.Comment = request.Note ?? "";
                historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

                _context.ApplicationForms.Update(applicationForm);
                _context.HistoryApplicationForms.Add(historyApplicationForm);              

                SendRejectEmailLeaveRequest(userRequester, leaveRequest, request.Note ?? "");

                await _context.SaveChangesAsync();
                return null;
            }

            //đơn này đã đến bộ phận hr và người approval là hr => complete
            if (applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR && roleClaims.Contains("HR"))
            {
                applicationForm.Id = applicationForm.Id;
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
                applicationForm.CurrentOrgUnitId = ORG_UNIT_ID_COMPLETE_DONE;

                historyApplicationForm.UserApproval = request.NameUserApproval;
                historyApplicationForm.ActionType = "APPROVAL";
                historyApplicationForm.Comment = request.Note ?? "";
                historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

                _context.ApplicationForms.Update(applicationForm);
                _context.HistoryApplicationForms.Add(historyApplicationForm);

                SendEmailSuccessLeaveRequest(userApprovalInWebSystem, leaveRequest, request.Note ?? "");

                await _context.SaveChangesAsync();
                return null;
            }

            string urlWaitApproval = $"{request.UrlFrontEnd}/leave/wait-approval";
            string bodyMail = $@"
                <h4>
                    <span>Duyệt đơn: </span>
                    <a href={urlWaitApproval}>{urlWaitApproval}</a>
                </h4>" + TemplateEmail.EmailContentLeaveRequest(leaveRequest, leaveRequest.TypeLeave, leaveRequest.TimeLeave) + "<br/>";

            List<string> toEmails = [];

            if (orgUnit == null || orgUnit?.ParentJobTitleId == null)
            {
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.WAIT_HR;
                applicationForm.CurrentOrgUnitId = (int)StatusApplicationFormEnum.WAIT_HR;

                historyApplicationForm.UserApproval = request.NameUserApproval;
                historyApplicationForm.ActionType = "APPROVAL";
                historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

                toEmails.Add(Global.EmailHRReceiveLeaveRequest);
            }
            else
            {
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                applicationForm.CurrentOrgUnitId = orgUnit?.ParentJobTitleId;

                historyApplicationForm.UserApproval = request.NameUserApproval;
                historyApplicationForm.ActionType = "APPROVAL";
                historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

                dynamic users = await _userService.GetMultipleUserViclockByOrgUnitId(orgUnit?.ParentJobTitleId);

                if (users is IEnumerable<object> list && list.Any())
                {
                    foreach (var user in users)
                    {
                        dynamic u = user;
                        string email = !string.IsNullOrWhiteSpace(u?.Email) ? u.Email : (!string.IsNullOrWhiteSpace(u?.NVEmail) ? u.NVEmail : "");
                        if (!string.IsNullOrEmpty(email))
                        {
                            toEmails.Add(email);
                        }
                    }
                }
            }

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string titleEmail = $"Đơn xin nghỉ phép - {leaveRequest.RequesterUserCode} - {leaveRequest.Name}";

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailRequestHasBeenSent(
                    toEmails,
                    null,
                    titleEmail,
                    bodyMail,
                    null,
                    true
                )
            );

            return null;
        }

        private void SendEmailSuccessLeaveRequest(Domain.Entities.User userRequester, Domain.Entities.LeaveRequest leaveRequest, string rejectionNote)
        {
            if (userRequester?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false")
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(userRequester?.Email))
            {
                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRejectLeaveRequest(
                        new List<string> { userRequester.Email },
                        null,
                        "Đơn xin nghỉ phép đã được đăng ký thành công",
                        TemplateEmail.EmailContentLeaveRequest(leaveRequest, leaveRequest.TypeLeave, leaveRequest.TimeLeave),
                        null,
                        true
                    )
                );
            }
        }

        private void SendRejectEmailLeaveRequest(Domain.Entities.User? userRequester, Domain.Entities.LeaveRequest leaveRequest, string rejectionNote)
        {
            if (userRequester?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false")
            {
                return;
            }

            string bodyMailReject = $@"<h4><span style=""color:red"">Lý do từ chối: {rejectionNote}</span></h4>" +
                            TemplateEmail.EmailContentLeaveRequest(leaveRequest, leaveRequest.TypeLeave, leaveRequest.TimeLeave) + "<br/>";

            if (!string.IsNullOrWhiteSpace(userRequester?.Email))
            {
                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRejectLeaveRequest(
                        new List<string> { userRequester.Email },
                        null,
                        "Đơn xin nghỉ phép đã bị từ chối",
                        bodyMailReject,
                        null,
                        true
                    )
                );
            }
        }
        public async Task<PagedResults<LeaveRequestDto>> GetHistoryLeaveRequestApproval(GetAllLeaveRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;
            string? UserCode = request?.UserCode;
            string? Keyword = request?.Keyword;

            var q = _context.LeaveRequests
                .Include(e => e.TypeLeave)
                .Include(e => e.TimeLeave)
                .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.HistoryApplicationForms)
                .Where(e => e.ApplicationForm != null && e.ApplicationForm.HistoryApplicationForms.Any(e => e.UserCodeApproval == UserCode));

            if (!string.IsNullOrWhiteSpace(Keyword))
            {
                q = q.Where(e => (e.Name != null && e.Name.Contains(Keyword)) ||  (e.RequesterUserCode != null && e.RequesterUserCode.Contains(Keyword)));
            }

            var totalItems = await q.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var dtos = LeaveRequestMapper.ToDtoList(pagedResult);

            var data = new PagedResults<LeaveRequestDto>
            {
                Data = dtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return data;
        }

        //public async Task<string> HrRegisterAllLeave(HrRegisterAllLeaveRequest request)
        //{
        //    var statusList = new[] {
        //        StatusLeaveRequestEnum.IN_PROCESS.ToString(),
        //        StatusLeaveRequestEnum.PENDING.ToString()
        //    };

        //    var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found");

        //    var result = await _context.ApprovalRequests
        //        .Where(e =>
        //            e.RequestType == "LEAVE_REQUEST" &&
        //            statusList.Contains(e.Status) && e.CurrentPositionId == (int)StatusLeaveRequestEnum.WAIT_HR
        //        )
        //        .Join(_context.LeaveRequests, approvalRequest => approvalRequest.RequestId, leave => leave.Id, (approvalRequest, leave) => new
        //        {
        //            ApprovalRequest = approvalRequest,
        //            LeaveRequest = leave
        //        })
        //        .ToListAsync();

        //    List<HistoryApplicationForm> actions = [];

        //    foreach (var item in result)
        //    {
        //        item.ApprovalRequest.Status = StatusLeaveRequestEnum.COMPLETED.ToString();

        //        actions.Add(new HistoryApplicationForm
        //        {
        //            ApprovalRequestId = item.ApprovalRequest.Id,
        //            ApproverUserCode = request?.UserCode,
        //            ApproverName = request?.UserName,
        //            Action = StatusLeaveRequestEnum.COMPLETED.ToString(),
        //            CreatedAt = DateTimeOffset.Now
        //        });

        //        _context.ApprovalRequests.Update(item.ApprovalRequest);

        //        var checkEmail = await _userService.GetEmailByUserCodeAndUserConfig(new List<string> { item.ApprovalRequest.RequesterUserCode ?? Global.EmailDefault });
        //        var firstEmail = checkEmail.FirstOrDefault();
        //        var email = firstEmail != null && !string.IsNullOrWhiteSpace(firstEmail.Email) ? firstEmail.Email : Global.EmailDefault;

        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailAsync(
        //                new List<string> { email },
        //                null,
        //                "Đơn xin nghỉ phép của bạn đã đăng ký thành công!",
        //                FormatContentMailLeaveRequest(item.LeaveRequest),
        //                null,
        //                true
        //            )
        //        );
        //    }

        //    _context.ApprovalActions.AddRange(actions);

        //    await _context.SaveChangesAsync();

        //    return "success" ?? "error";
        //}

        //public async Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request)
        //{
        //    if (request.Leaves != null && request.Leaves.Count > 0)
        //    {
        //        var userCodeWriteRequester = request.Leaves.First().WriteLeaveUserCode;

        //        //lấy thông tin người viết đơn cho người khác
        //        var userWriteRequester = await _context.Users
        //            .Where(e => e.UserCode == userCodeWriteRequester)
        //            .Select(e => new
        //            {
        //                e.UserCode,
        //                e.PositionId,
        //                e.Email
        //            })
        //            .FirstOrDefaultAsync() ?? throw new NotFoundException("User not found!");

        //        //luồng duyệt
        //        var approvalFlow = await _context.ApprovalFlows
        //            .Where(e => e.FromPosition == userWriteRequester.PositionId)
        //            .Select(x => new
        //            {
        //                x.ToPosition
        //            })
        //            .FirstOrDefaultAsync();

        //        //nếu hết người duyệt, gửi đến HR
        //        if (approvalFlow == null)
        //        {
        //            foreach (var itemLeave in request.Leaves)
        //            {
        //                //lưu bảng leave_requests và approval_requests
        //                var newLeave = LeaveRequestMapper.ToEntity(itemLeave);
        //                _context.LeaveRequests.Add(newLeave);

        //                _context.ApprovalRequests.Add(new Domain.Entities.ApplicationForm
        //                {
        //                    RequesterUserCode = itemLeave.RequesterUserCode,
        //                    RequestType = "LEAVE_REQUEST",
        //                    RequestId = newLeave.Id,
        //                    Status = "IN_PROCESS",
        //                    CurrentPositionId = (int)StatusLeaveRequestEnum.WAIT_HR,
        //                    CreatedAt = DateTimeOffset.Now
        //                });

        //                await _context.SaveChangesAsync();
        //            }

        //            return true;
        //        }

        //        //lấy những người duyệt tiếp theo
        //        var userHaveNextPosition = await _context.Users
        //            .Where(e => e.PositionId == approvalFlow.ToPosition)
        //            .Select(x => new
        //            {
        //                x.PositionId,
        //                x.Email
        //            })
        //            .ToListAsync();

        //        foreach (var itemLeave in request.Leaves)
        //        {
        //            var newLeave = LeaveRequestMapper.ToEntity(itemLeave);
        //            _context.LeaveRequests.Add(newLeave);

        //            _context.ApprovalRequests.Add(new Domain.Entities.ApplicationForm
        //            {
        //                RequesterUserCode = itemLeave.RequesterUserCode,
        //                RequestType = "LEAVE_REQUEST",
        //                RequestId = newLeave.Id,
        //                Status = "PENDING",
        //                CurrentPositionId = approvalFlow.ToPosition,
        //                CreatedAt = DateTimeOffset.Now
        //            });
        //        }

        //        //gửi email đến người duyệt tiếp theo
        //        var toEmails = new List<string>();
        //        foreach (var item in userHaveNextPosition)
        //        {
        //            toEmails.Add(!string.IsNullOrWhiteSpace(item.Email) ? item.Email : Global.EmailDefault);
        //        }

        //        //gửi email cho người xin nghỉ, check kiểm tra người dùng có config nhận thông báo email không
        //        //nếu như check là k nhận thông báo thì k gửi, ngược lại các trường hợp sẽ gửi
        //        var ccEmails = new List<string>();
        //        var getEmailByUserCodeAndUserConfig = await _userService.GetEmailByUserCodeAndUserConfig([.. request.Leaves.Select(x => x.RequesterUserCode)]);

        //        foreach (var item in getEmailByUserCodeAndUserConfig)
        //        {
        //            ccEmails.Add(!string.IsNullOrWhiteSpace(item.Email) ? item.Email : Global.EmailDefault);
        //        }

        //        ccEmails.Add(!string.IsNullOrWhiteSpace(userWriteRequester.Email) ? userWriteRequester.Email : Global.EmailDefault);

        //        string? requester = request.Leaves?.Count == 1 ? request.Leaves?.FirstOrDefault()?.RequesterUserCode : $"{request.Leaves?.Count} người";

        //        string subject = $"Đơn xin nghỉ phép - {requester}";

        //        string urlWaitApproval = $"{request.Leaves?.FirstOrDefault()?.UrlFrontend}/leave/wait-approval";

        //        string bodyMail = $@"
        //            <h4>
        //                <span>Duyệt đơn: </span>
        //                <a href={urlWaitApproval}>{urlWaitApproval}</a>
        //            </h4>";

        //        if (request.Leaves != null && request.Leaves.Count > 0)
        //        {
        //            foreach (var itemLeave in request.Leaves)
        //            {
        //                bodyMail += FormatContentMailLeaveRequest(LeaveRequestMapper.ToEntity(itemLeave)) + "<br/>";
        //            }
        //        }

        //        BackgroundJob.Enqueue<IEmailService>(job => 
        //            job.SendEmailAsync(
        //                toEmails, 
        //                ccEmails,
        //                subject,
        //                bodyMail,
        //                null,
        //                true
        //            )
        //        );

        //        await _context.SaveChangesAsync();

        //        return true;
        //    }

        //    throw new Exception("Không có người nào xin nghỉ phép");
        //}
    }
}
