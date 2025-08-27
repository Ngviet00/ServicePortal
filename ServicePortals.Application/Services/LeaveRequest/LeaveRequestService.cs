using System.Data;
using System.Security.Claims;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.LeaveRequest
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ExcelService _excelService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int PARENT_ORG_POSITION_GN = 1;

        public LeaveRequestService(
            ApplicationDbContext context,
            IUserService userService,
            ExcelService excelService,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _userService = userService;
            _excelService = excelService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Lấy danh sách nhưng đơn nghỉ phép của user, nếu như request gửi lên là in process thì hthi những đơn có trạng thái là in process hoặc wait hr
        /// </summary>
        public async Task<PagedResults<Domain.Entities.LeaveRequest>> GetAll(GetAllLeaveRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;
            int? status = request.Status;
            string? UserCode = request?.UserCode;

            var query = _context.LeaveRequests
                .Where(l => (l.UserCodeRequestor == UserCode || l.UserCodeCreated == UserCode) &&
                            l.ApplicationForm != null &&
                            (
                                status == (int)StatusApplicationFormEnum.IN_PROCESS
                                    ? l.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS || l.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR
                                    : l.ApplicationForm.RequestStatusId == status
                            )
                );

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedResult = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new Domain.Entities.LeaveRequest
                {
                    Id = x.Id,
                    Code = x.Code,
                    UserCodeRequestor = x.UserCodeRequestor,
                    UserNameRequestor = x.UserNameRequestor,
                    Position = x.Position,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    Reason = x.Reason,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    TimeLeave = x.TimeLeave,
                    TypeLeave = x.TypeLeave,
                    OrgUnit = x.OrgUnit,
                    User = x.User,
                    ApplicationForm = x.ApplicationForm != null ? new ApplicationForm
                    {
                        Id = x.ApplicationForm.Id,
                        UserCodeRequestor = x.ApplicationForm.UserCodeRequestor,
                        UserNameRequestor = x.ApplicationForm.UserNameRequestor,
                        OrgPositionId = x.ApplicationForm.OrgPositionId,
                        CreatedAt = x.ApplicationForm.CreatedAt,
                        RequestType = x.ApplicationForm.RequestType,
                        RequestStatus = x.ApplicationForm.RequestStatus,
                        HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(h => h.CreatedAt).ToList(),
                    } : null
                })
                .ToListAsync();

            var countPending = await _context.LeaveRequests
                .Include(e => e.ApplicationForm)
                .Where(e => (e.UserCodeRequestor == UserCode || e.UserCodeCreated == UserCode) && e.ApplicationForm != null && e.ApplicationForm.RequestStatusId == 1) //1 pending
                .CountAsync();

            var countInProcess = await _context.LeaveRequests
                .Include(e => e.ApplicationForm)
                .Where(e => (e.UserCodeRequestor == UserCode || e.UserCodeCreated == UserCode) && e.ApplicationForm != null &&
                    (
                        e.ApplicationForm.RequestStatusId == 2 || e.ApplicationForm.RequestStatusId == 4
                    )
                )
                .CountAsync();

            return new PagedResults<Domain.Entities.LeaveRequest>
            {
                Data = pagedResult,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CountPending = countPending,
                CountInProcess = countInProcess,
            };
        }

        public async Task<Domain.Entities.LeaveRequest> GetById(Guid? id)
        {
            var leaveRequest = await _context.LeaveRequests
                .Select(x => new Domain.Entities.LeaveRequest
                {
                    Id = x.Id,
                    Code = x.Code,
                    UserCodeRequestor = x.UserCodeRequestor,
                    UserNameRequestor = x.UserNameRequestor,
                    Position = x.Position,
                    FromDate = x.FromDate,
                    ToDate = x.ToDate,
                    Reason = x.Reason,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt,
                    TimeLeave = x.TimeLeave,
                    TypeLeave = x.TypeLeave,
                    OrgUnit = x.OrgUnit,
                    User = x.User,
                    ApplicationFormId = x.ApplicationFormId,
                    ApplicationForm = x.ApplicationForm != null ? new ApplicationForm
                    {
                        Id = x.ApplicationForm.Id,
                        UserCodeRequestor = x.ApplicationForm.UserCodeRequestor,
                        UserNameRequestor = x.ApplicationForm.UserNameRequestor,
                        OrgPositionId = x.ApplicationForm.OrgPositionId,
                        CreatedAt = x.ApplicationForm.CreatedAt,
                        RequestType = x.ApplicationForm.RequestType,
                        RequestTypeId = x.ApplicationForm.RequestTypeId,
                        RequestStatus = x.ApplicationForm.RequestStatus,
                        HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(h => h.CreatedAt).ToList(),
                    } : null
                })
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NotFoundException("Leave request not found!");

            return leaveRequest;
        }

        /// <summary>
        /// Tạo đơn nghỉ phép, phải có orgUnitId, lấy luồng duyệt từ bảng workflowstep, sau đó gửi email cho người tiếp theo, mặc định k có ai cấp trên thì gửi thẳng hr
        /// </summary>
        public async Task<object> Create(CreateLeaveRequest request)
        {
            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId);

            if (orgPosition == null)
            {
                throw new ValidationException("Thông tin chưa được cập nhật, vui lòng liên hệ với HR!");
            }

            if (request.DepartmentId == null || request.DepartmentId < 0)
            {
                throw new NotFoundException("Department is invalid!");
            }

            //lấy thông tin người dùng hiện tại
            var requesterUser = await _context.Users.Include(e => e.UserConfigs).FirstOrDefaultAsync(e => e.UserCode == request.UserCodeRequestor) ?? throw new NotFoundException("User not found!");

            int requestStatusApplicationForm = -1;
            int? nextOrgPositionId = orgPosition.ParentOrgPositionId;

            bool isSendHr = false;

            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

            //send to hr, không có parent, có approval flow và approval flow is final = true, next org position = 1 là general manager
            if (nextOrgPositionId == null || (approvalFlows != null && approvalFlows.IsFinal == true) || nextOrgPositionId == PARENT_ORG_POSITION_GN) //next org position = 1 là của GM
            {
                isSendHr = true;
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR; //send hr
                nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ; //send hr
            }
            else if (approvalFlows != null)
            {
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
                nextOrgPositionId = approvalFlows.ToOrgPositionId;
            }
            else
            {
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
            }

            ApplicationForm newApplicationForm = new()
            {
                Id = Guid.NewGuid(),
                UserCodeRequestor = request.UserCodeRequestor,
                UserNameRequestor = request.UserNameRequestor,
                RequestTypeId = (int)RequestTypeEnum.LEAVE_REQUEST,
                RequestStatusId = requestStatusApplicationForm,
                OrgPositionId = nextOrgPositionId,
                CreatedAt = DateTimeOffset.Now
            };
            _context.ApplicationForms.Add(newApplicationForm);

            Domain.Entities.LeaveRequest newLeaveRequest = new()
            {
                Code = Helper.GenerateFormCode("LR"),
                ApplicationFormId = newApplicationForm.Id,
                UserCodeRequestor = request.UserCodeRequestor,
                UserNameRequestor = request.UserNameRequestor,
                DepartmentId = request.DepartmentId,
                Position = request.Position,
                UserCodeCreated = request.WriteLeaveUserCode,
                CreatedBy = request.UserNameWriteLeaveRequest,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                TimeLeaveId = request.TimeLeaveId,
                TypeLeaveId = request.TypeLeaveId,
                Reason = request.Reason,
                CreatedAt = DateTimeOffset.Now
            };
            _context.LeaveRequests.Add(newLeaveRequest);

            await _context.SaveChangesAsync();

            var latestNewLeaveRequest = await GetById(newLeaveRequest.Id);

            string templateEmail = TemplateEmail.EmailContentLeaveRequest(latestNewLeaveRequest);

            //gửi email thông tin cho chính mình
            if (!(requesterUser?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false"))
            {
                var emailCurrentUser = requesterUser?.Email ?? "";

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        new List<string> { emailCurrentUser },
                        null,
                        "Đơn xin nghỉ phép đã được gửi",
                        templateEmail,
                        null,
                        true
                    )
                );
            }

            //gửi email thông tin cho người tiếp theo
            string urlWaitApproval = $"{request.UrlFrontend}/leave/wait-approval";
            string emailWithUrlApproval = $@"
                    <h4>
                        <span>Duyệt đơn: </span>
                        <a href={urlWaitApproval}>{urlWaitApproval}</a>
                    </h4>" + templateEmail + "<br/>"
            ;

            if (isSendHr) //gửi email cho HR
            {
                var hrHavePermissionMngLeaveRequest = await GetHrWithManagementLeavePermission();

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        hrHavePermissionMngLeaveRequest.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Đơn xin nghỉ phép",
                        emailWithUrlApproval,
                        null,
                        true
                    )
                );
            }
            else //lấy email của những người tiếp theo và gửi email cho họ
            {
                var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        receiveUser.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Đơn xin nghỉ phép",
                        emailWithUrlApproval,
                        null,
                        true
                    )
                );
            }

            return true;
        }

        public async Task<object> Update(Guid id, LeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            leaveRequest.Id = leaveRequest.Id;
            leaveRequest.DepartmentId = dto.DepartmentId;
            leaveRequest.Position = dto.Position;

            leaveRequest.FromDate = dto.FromDate;
            leaveRequest.ToDate = dto.ToDate;

            leaveRequest.TimeLeaveId = dto.TimeLeaveId;
            leaveRequest.TypeLeaveId = dto.TypeLeaveId;
            leaveRequest.Reason = dto.Reason;
            leaveRequest.UpdateAt = DateTimeOffset.Now;

            _context.LeaveRequests.Update(leaveRequest);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> Delete(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == leaveRequest.ApplicationFormId);

            var historyApplicationForms = await _context.HistoryApplicationForms.Where(e => applicationForm != null && e.ApplicationFormId == applicationForm.Id).ToListAsync();

            _context.HistoryApplicationForms.RemoveRange(historyApplicationForms);

            _context.LeaveRequests.Remove(leaveRequest);

            _context.ApplicationForms.Remove(applicationForm);

            var requestOfLeave = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == leaveRequest.ApplicationFormId) ?? throw new NotFoundException("Leave request not found!");

            _context.ApplicationForms.Remove(requestOfLeave);

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Hàm duyệt đơn nghỉ phép, cần orgUnitId, lấy luồng duyệt theo workflowstep nếu approval thì gửi đến người tiếp theo, hoặc k có người tiếp thì gửi đến hr
        /// khi approval hoặc reject thì sẽ gửi email đến người đó
        public async Task<object> Approval(ApprovalRequest request)
        {
            var userClaim = _httpContextAccessor.HttpContext.User;

            var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var connection = (SqlConnection)_context.CreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId);

            if (orgPosition == null)
            {
                throw new ValidationException("Thông tin chưa được cập nhật, vui lòng liên hệ với HR!");
            }

            //var delegatedTempUser = await _context.DelegatedTemps.FirstOrDefaultAsync(e => e.RequestTypeId == 1 && e.TempUserCode == request.UserCodeApproval && e.IsActive == true);

            //if (delegatedTempUser != null)
            //{
            //    orgUnitIdCurrentUser = delegatedTempUser.MainOrgUnitId;
            //}
            //else
            //{
            //orgPositionIdCurrentUser = await connection.QueryFirstOrDefaultAsync<int>($@"SELECT OrgUnitID FROM {Global.DbViClock}.dbo.tblNhanVien AS NV WHERE NVMaNV = @UserCode", new
            //{
            //    UserCode = request.UserCodeApproval
            //});
            ////}

            //if (orgPositionId == 0 || orgPositionId == null) //nếu hr chưa set org unit id, user sẽ k thể đăng ký nghỉ phép đc
            //{
            //    throw new ValidationException("Thông tin chưa được cập nhật, vui lòng liên hệ với HR!");
            //}

            var leaveRequest = await GetById((Guid)(request.LeaveRequestId));

            var userRequester = leaveRequest.User;

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == leaveRequest.ApplicationFormId);

            if (applicationForm == null)
            {
                throw new NotFoundException("Application form is not found, please check again");
            }

            var historyApplicationForm = new HistoryApplicationForm
            {
                ApplicationFormId = applicationForm.Id,
                CreatedAt = DateTimeOffset.Now
            };

            int requestStatusApplicationForm = -1;
            int? nextOrgPositionId = orgPosition.ParentOrgPositionId;

            //bool isComplete = false;
            bool isSendHr = false;

            //lấy danh sách workflow của người hiện tại, check xem user có custom workflow không
            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

            if (approvalFlows != null)
            {
                if (approvalFlows.IsFinal == true)
                {
                    //send to hr
                    isSendHr = true;
                    requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR;
                    nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
                }
                else
                {
                    requestStatusApplicationForm = (int)StatusApplicationFormEnum.IN_PROCESS;
                    nextOrgPositionId = approvalFlows.ToOrgPositionId;
                }
            }
            else if (nextOrgPositionId == null)
            {
                //send to hr
                isSendHr = true;
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR;
                nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
            }
            else
            {
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.IN_PROCESS;
            }

            //case reject
            if (request.Status == false)
            {

                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
                applicationForm.UpdatedAt = DateTimeOffset.Now;
                _context.ApplicationForms.Update(applicationForm);

                historyApplicationForm.UserNameApproval = request.UserNameApproval;
                historyApplicationForm.Action = "REJECT";
                historyApplicationForm.Note = request.Note ?? "";
                historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

                _context.HistoryApplicationForms.Add(historyApplicationForm);

                SendRejectEmailLeaveRequest(userRequester, leaveRequest, request.Note ?? "");

                await _context.SaveChangesAsync();
                return true;
            }

            applicationForm.Id = applicationForm.Id;
            applicationForm.RequestStatusId = requestStatusApplicationForm;
            applicationForm.OrgPositionId = nextOrgPositionId;

            historyApplicationForm.UserNameApproval = request.UserNameApproval;
            historyApplicationForm.Action = "APPROVAL";
            historyApplicationForm.Note = request.Note ?? "";
            historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string templateEmail = TemplateEmail.EmailContentLeaveRequest(leaveRequest);

            //gửi email thông tin cho người tiếp theo
            string urlWaitApproval = $"{request.UrlFrontend}/leave/wait-approval";
            string emailWithUrlApproval = $@"
                    <h4>
                        <span>Duyệt đơn: </span>
                        <a href={urlWaitApproval}>{urlWaitApproval}</a>
                    </h4>" + templateEmail + "<br/>"
            ;

            if (isSendHr)
            {
                var hrHavePermissionMngLeaveRequest = await GetHrWithManagementLeavePermission();

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        hrHavePermissionMngLeaveRequest.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Đơn xin nghỉ phép",
                        emailWithUrlApproval,
                        null,
                        true
                    )
                );
            }
            else
            {
                var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        receiveUser.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Đơn xin nghỉ phép",
                        emailWithUrlApproval,
                        null,
                        true
                    )
                );
            }

            return true;
        }

        //hàm send email khi mà approved bước cuối cùng thành công
        private void SendEmailSuccessLeaveRequest(Domain.Entities.User userRequester, Domain.Entities.LeaveRequest leaveRequest)
        {
            if (userRequester?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false")
            {
                return;
            }

            if (!string.IsNullOrWhiteSpace(userRequester?.Email))
            {
                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRejectLeaveRequest(
                        new List<string> { userRequester.Email ?? Global.EmailDefault },
                        null,
                        "Đơn xin nghỉ phép đã được đăng ký thành công",
                        TemplateEmail.EmailContentLeaveRequest(leaveRequest),
                        null,
                        true
                    )
                );
            }
        }

        //hàm send email khi mà bị từ chối reject
        private void SendRejectEmailLeaveRequest(Domain.Entities.User? userRequester, Domain.Entities.LeaveRequest leaveRequest, string rejectionNote)
        {
            if (userRequester?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false")
            {
                return;
            }

            string bodyMailReject = $@"<h4><span style=""color:red"">Lý do từ chối: {rejectionNote}</span></h4>" +
                            TemplateEmail.EmailContentLeaveRequest(leaveRequest) + "<br/>";

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

        /// <summary>
        /// cập nhật những người có quyền quản lý nghỉ phép, có thể đăng ký nghỉ phép hộ
        /// </summary>
        public async Task<object> UpdateUserHavePermissionCreateMultipleLeaveRequest(List<string> UserCodes)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.create_multiple_leave_request")
                ?? throw new NotFoundException("Permission not found");

            var userPermissions = await _context.UserPermissions.Where(e => e.PermissionId == permission.Id).ToListAsync();

            var currentUserCodes = userPermissions.Select(e => e.UserCode).ToHashSet();
            var newUserCodesSet = UserCodes.ToHashSet();
            var toRemove = userPermissions.Where(e => !newUserCodesSet.Contains(e?.UserCode ?? "")).ToList();
            var toAdd = UserCodes.Where(code => !currentUserCodes.Contains(code)).Select(code => new UserPermission { PermissionId = permission.Id, UserCode = code });

            _context.UserPermissions.RemoveRange(toRemove);
            _context.UserPermissions.AddRange(toAdd);

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// lấy những người có quyền quản lý nghỉ phép, có thể đăng ký nghỉ phép hộ
        /// </summary>
        public async Task<object> GetUserCodeHavePermissionCreateMultipleLeaveRequest()
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.create_multiple_leave_request");

            if (permission == null)
            {
                throw new NotFoundException("Permission not found");
            }

            return await _context.UserPermissions.Where(e => e.PermissionId == permission.Id).Select(e => e.UserCode).ToListAsync();
        }

        /// <summary>
        /// Chọn những vị trí được quản lý nghỉ phép cho người dùng, vd: người a quản lý tổ A, tổ B
        /// </summary>
        public async Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request)
        {
            var userMngOrgUnitIds = await _context.UserMngOrgUnitId
                .Where(e => e.UserCode == request.UserCode && e.ManagementType == "MNG_LEAVE_REQUEST")
                .ToListAsync();

            var existingIds = userMngOrgUnitIds.Select(e => e.OrgUnitId).ToHashSet();
            var newIds = request.OrgUnitIds.ToHashSet();

            _context.UserMngOrgUnitId.RemoveRange(userMngOrgUnitIds.Where(e => !newIds.Contains((int)(e?.OrgUnitId))));

            _context.UserMngOrgUnitId.AddRange(request.OrgUnitIds
                .Where(id => !existingIds.Contains(id))
                .Select(id => new UserMngOrgUnitId
                {
                    UserCode = request.UserCode,
                    OrgUnitId = id,
                    ManagementType = "MNG_LEAVE_REQUEST"
                })
            );

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Lấy những vị trí được quản lý nghỉ phép theo người dùng, vd: ID: 1 (tổ a), ID: 2 (tổ b)
        /// </summary>
        public async Task<object> GetOrgUnitIdAttachedByUserCode(string userCode)
        {
            var results = await _context.UserMngOrgUnitId
                .Where(e => e.UserCode == userCode && e.ManagementType == "MNG_LEAVE_REQUEST")
                .Select(e => e.OrgUnitId)
                .ToListAsync();

            return results;
        }

        /// <summary>
        /// Tìm kiếm người xin nghỉ phép ở màn tạo nghỉ phép hộ, vd: người a có thể tìm kiếm người a,b,c, k thể tìm kiếm người d, được thiết lập ở màn ql nghỉ phép của HR
        /// </summary>
        public async Task<object> SearchUserRegisterLeaveRequest(SearchUserRegisterLeaveRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserCodeMng", request.UserCodeRegister, DbType.String, ParameterDirection.Input);
            parameters.Add("@Type", "MNG_LEAVE_REQUEST", DbType.String, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);

            var result = await _context.Database.GetDbConnection()
                .QueryFirstOrDefaultAsync<object>(
                    "dbo.SearchUserRegisterLeaveRequest",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            if (result != null)
            {
                return result;
            }
            else
            {
                throw new ValidationException("Bạn chưa có quyền đăng ký nghỉ phép cho người này, liên hệ HR");
            }
        }

        /// <summary>
        /// xin nghỉ phép cho nghiều người khác, gửi cho cấp trên của người tạo đơn nghỉ phép, vd: a viết phép nghỉ cho b, c -> gửi cho cấp trên của a
        /// </summary>
        public async Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request)
        {
            int? orgPositionId = request.OrgPositionId;
            string? userCode = request.UserCode;
            string? urlFrontEnd = request.UrlFrontEnd;

            if (orgPositionId == null)
            {
                throw new ValidationException("Dữ liệu vị trí chưa được cập nhật, liên hệ với HR!");
            }

            if (request.Leaves == null || request.Leaves != null && request.Leaves.Count == 0)
            {
                throw new ValidationException("Không có người nào xin nghỉ phép, vui lòng check lại!");
            }

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException("Dữ liệu vị trí chưa được cập nhật, liên hệ với HR!");
            var requesterUser = await _context.Users.Include(e => e.UserConfigs).FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found!");

            int requestStatusApplicationForm = -1;
            int? nextOrgPositionId = orgPosition.ParentOrgPositionId;
            bool isSendHr = false;

            var timeLeaves = await _context.TimeLeaves.ToListAsync();
            var typeLeaves = await _context.TypeLeaves.ToListAsync();
            var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

            if (nextOrgPositionId == null || (approvalFlows != null && approvalFlows.IsFinal == true) || nextOrgPositionId == PARENT_ORG_POSITION_GN) //next org position = 1 là của GM
            {
                isSendHr = true;
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR; //send hr
                nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ; //send hr
            }
            else if (approvalFlows != null)
            {
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
                nextOrgPositionId = approvalFlows.ToOrgPositionId;
            }
            else
            {
                requestStatusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
            }

            List<ApplicationForm> applicationForms = [];
            List<Domain.Entities.LeaveRequest> leaveRequests = [];

            List<string> userCodes = [userCode ?? ""];

            string urlWaitApproval = $"{urlFrontEnd}/leave/wait-approval";

            string emailLinkApproval = $@"
                <h4>
                    <span>Duyệt đơn: </span>
                    <a href={urlWaitApproval}>{urlWaitApproval}</a>
                </h4>";

            string bodyMail = $@"";

            foreach (var itemLeave in request.Leaves)
            {
                userCodes.Add(itemLeave.UserCodeRequestor ?? "");

                var newApplicationForm = new ApplicationForm
                {
                    Id = Guid.NewGuid(),
                    UserCodeRequestor = itemLeave.UserCodeRequestor,
                    UserNameRequestor = itemLeave.UserNameRequestor,
                    RequestTypeId = (int)RequestTypeEnum.LEAVE_REQUEST,
                    RequestStatusId = requestStatusApplicationForm,
                    OrgPositionId = nextOrgPositionId,
                    CreatedAt = DateTimeOffset.Now
                };
                _context.ApplicationForms.Add(newApplicationForm);

                var newLeave = new Domain.Entities.LeaveRequest
                {
                    Code = Helper.GenerateFormCode("LR"),
                    ApplicationFormId = newApplicationForm.Id,
                    UserCodeRequestor = itemLeave.UserCodeRequestor,
                    UserNameRequestor = itemLeave.UserNameRequestor,
                    DepartmentId = itemLeave.DepartmentId,
                    Position = itemLeave.Position,
                    UserCodeCreated = itemLeave.WriteLeaveUserCode,
                    CreatedBy = itemLeave.UserNameWriteLeaveRequest,
                    FromDate = itemLeave.FromDate,
                    ToDate = itemLeave.ToDate,
                    TimeLeaveId = itemLeave.TimeLeaveId,
                    TypeLeaveId = itemLeave.TypeLeaveId,
                    Reason = itemLeave.Reason,
                    CreatedAt = DateTimeOffset.Now
                };

                _context.LeaveRequests.Add(newLeave);

                newLeave.TypeLeave = typeLeaves.FirstOrDefault(e => e.Id == itemLeave.TypeLeaveId);
                newLeave.TimeLeave = timeLeaves.FirstOrDefault(e => e.Id == itemLeave.TimeLeaveId);
                newLeave.OrgUnit = orgUnits.FirstOrDefault(e => e.Id == itemLeave.DepartmentId);

                bodyMail += TemplateEmail.EmailContentLeaveRequest(newLeave) + "<br/>";
            }

            await _context.SaveChangesAsync();

            List<string> emailSendNoti = (List<string>)await _context.Database.GetDbConnection().QueryAsync<string>($@"
                    SELECT COALESCE(NULLIF(U.Email, ''), NV.NVEmail, '') AS Email FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                    LEFT JOIN {Global.DbWeb}.dbo.user_configs AS UC ON NV.NVMaNV = UC.UserCode 
                    LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
                    WHERE NV.NVMaNV IN @UserCodes
                    AND (UC.UserCode IS NULL OR (UC.UserCode IS NOT NULL AND UC.[Key] = 'RECEIVE_MAIL_LEAVE_REQUEST' AND UC.Value = 'true'))
                    ", new { UserCodes = userCodes.Distinct().ToList() });

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailRequestHasBeenSent(
                    emailSendNoti.ToList(),
                    null,
                    "Đơn xin nghỉ phép đã được gửi",
                    bodyMail,
                    null,
                    true
                )
            );

            //gửi email cho hr
            if (isSendHr)
            {
                var hrHavePermissionMngLeaveRequest = await GetHrWithManagementLeavePermission();

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        hrHavePermissionMngLeaveRequest.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Đơn xin nghỉ phép",
                        emailLinkApproval + bodyMail,
                        null,
                        true
                    )
                );
            }
            else //gửi email cho người duyệt tiếp theo
            {
                var userNextOrgPosition = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailManyPeopleLeaveRequest(
                        userNextOrgPosition.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Đơn xin nghỉ phép",
                        emailLinkApproval + bodyMail,
                        null,
                        true
                    )
                );
            }

            return true;
        }

        /// <summary>
        /// Hàm HR đăng ký nghỉ phép tất cả, lấy những request có trạng tháo là wait hr và vị trí của HR mặc định là -10
        /// </summary>
        public async Task<object> HrRegisterAllLeave(HrRegisterAllLeaveRequest request)
        {
            var parsedIds = request.LeaveRequestIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                .Where(g => g != null)
                .Select(g => g.Value)
                .ToList();

            var leaveRequestsWaitHrApproval = await _context.LeaveRequests
                .AsNoTrackingWithIdentityResolution()
                .Include(e => e.OrgUnit)
                .Include(e => e.TimeLeave)
                .Include(e => e.TypeLeave)
                .Include(e => e.ApplicationForm)
                .Include(e => e.User)
                    .ThenInclude(e => e.UserConfigs)
                .Where(e =>
                    e.ApplicationForm != null &&
                    e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR &&
                    e.ApplicationForm.OrgPositionId == (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ &&
                    parsedIds.Contains(e.Id)
                )
                .ToListAsync();

            foreach (var itemLeave in leaveRequestsWaitHrApproval)
            {
                var applicationForm = itemLeave.ApplicationForm;

                if (applicationForm != null)
                {
                    applicationForm.Id = applicationForm.Id;
                    applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
                    applicationForm.OrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;

                    _context.ApplicationForms.Update(applicationForm);

                    var historyApplicationForm = new HistoryApplicationForm
                    {
                        ApplicationFormId = applicationForm.Id,
                        UserNameApproval = request.UserName,
                        Action = "APPROVAL",
                        UserCodeApproval = request.UserCode,
                        CreatedAt = DateTimeOffset.Now
                    };

                    _context.HistoryApplicationForms.Add(historyApplicationForm);

                    if (itemLeave.User != null)
                    {
                        SendEmailSuccessLeaveRequest(itemLeave.User, itemLeave);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Lấy danh sách những hr có quyền quản lý nghỉ phép
        /// </summary>
        public async Task<List<HrMngLeaveRequestResponse>> GetHrWithManagementLeavePermission()
        {
            var permissionHrMngLeaveRequest = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

            if (permissionHrMngLeaveRequest == null)
            {
                throw new NotFoundException("Permission hr manage leave request not found");
            }

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var userCodePerission = await _context.UserPermissions.Where(e => e.PermissionId == permissionHrMngLeaveRequest.Id).Select(e => e.UserCode).ToListAsync();

            var sql = $@"
                SELECT
                     NV.NVMaNV,
                     {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen,
                     BP.BPMa,
                     {Global.DbViClock}.dbo.funTCVN2Unicode(BP.BPTen) as BPTen,
                     COALESCE(NULLIF(Email, ''), NVEmail, '') AS Email
                FROM vs_new.dbo.tblNhanVien AS NV
                LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan as BP ON NV.NVMaBP = BP.BPMa
                LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
                WHERE NV.NVMaNV IN @userCodePerission
            ";

            var param = new
            {
                userCodePerission = userCodePerission
            };

            var result = await connection.QueryAsync<HrMngLeaveRequestResponse>(sql, param);

            return (List<HrMngLeaveRequestResponse>)result;
        }

        /// <summary>
        /// Thêm quyền hr quản lý nghỉ phép
        /// </summary>
        public async Task<object> UpdateHrWithManagementLeavePermission(List<string> UserCode)
        {
            var permissionHrMngLeaveRequest = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

            if (permissionHrMngLeaveRequest == null)
            {
                throw new NotFoundException("Permission hr manage leave request not found");
            }

            var oldUserPermissionsMngTKeeping = await _context.UserPermissions.Where(e => e.PermissionId == permissionHrMngLeaveRequest.Id).ToListAsync();

            _context.UserPermissions.RemoveRange(oldUserPermissionsMngTKeeping);

            List<UserPermission> newUserPermissions = new List<UserPermission>();

            foreach (var code in UserCode)
            {
                newUserPermissions.Add(new UserPermission
                {
                    UserCode = code,
                    PermissionId = permissionHrMngLeaveRequest.Id
                });
            }

            _context.UserPermissions.AddRange(newUserPermissions);

            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Hàm HR export leave request
        /// </summary>
        public async Task<byte[]> HrExportExcelLeaveRequest(List<string> leaveRequestIds)
        {
            var parsedIds = leaveRequestIds
                .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
                .Where(guid => guid.HasValue)
                .Select(guid => guid.Value)
                .ToList();

            var leaveRequestsWaitHrApproval = await _context.LeaveRequests
                .Include(e => e.TimeLeave)
                .Include(e => e.TypeLeave)
                .Where(e =>
                    e.ApplicationForm != null &&
                    e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR &&
                    e.ApplicationForm.OrgPositionId == (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ &&
                    parsedIds.Contains(e.Id)
                )
                .ToListAsync();

            return _excelService.ExportLeaveRequestToExcel(leaveRequestsWaitHrApproval);
        }
    }
}
