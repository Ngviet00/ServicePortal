using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortal.Applications.Modules.LeaveRequest.DTO.Requests;
using ServicePortals.Application.Common;

//using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Dtos.Purchase.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using GroupByDepartment = ServicePortals.Application.Dtos.LeaveRequest.Responses.GroupByDepartment;
using GroupByMonth = ServicePortals.Application.Dtos.LeaveRequest.Responses.GroupByMonth;
using GroupByTotal = ServicePortals.Application.Dtos.LeaveRequest.Responses.GroupByTotal;
using GroupRecentList = ServicePortals.Application.Dtos.LeaveRequest.Responses.GroupRecentList;

namespace ServicePortals.Application.Services.LeaveRequest
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly ExcelService _excelService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public LeaveRequestService(
            ApplicationDbContext context,
            IUserService userService,
            ExcelService excelService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration
        )
        {
            _context = context;
            _userService = userService;
            _excelService = excelService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo đơn xin nghỉ phép, hàm này có thể import nhập từ excel hoặc nhập tay
        /// </summary>
        public async Task<object> Create(CreateLeaveRequest request)
        {
            int orgPositionId = request.OrgPositionId ?? 0;

            if (orgPositionId <= 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            List<ApplicationFormItem> applicationFormItems = [];
            List<Domain.Entities.LeaveRequest> leaveRequests = [];
            List<string> userCodeReceiveEmail = [];

            var nextOrgPositionAndStatus = await GetNextOrgPositionAndStatusLeaveRequest(orgPositionId);

            int nextOrgPositionId = nextOrgPositionAndStatus.nextOrgPositionId;
            int status = nextOrgPositionAndStatus.status;

            var typeLeave = await _context.TypeLeaves.ToListAsync();
            var timeLeave = await _context.TimeLeaves.ToListAsync();
            var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            var newApplicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                Code = Helper.GenerateFormCode("LR"),
                RequestTypeId = (int)RequestTypeEnum.LEAVE_REQUEST,
                RequestStatusId = status,
                UserCodeCreatedBy = request.UserCodeCreated,
                OrgPositionId = nextOrgPositionId,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTimeOffset.Now
            };

            var newHistoryApplicationForm = new HistoryApplicationForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = newApplicationForm.Id,
                Action = "Created",
                ActionBy = request.CreatedBy,
                ActionAt = DateTimeOffset.Now,
            };

            string emailLinkApproval = $@"
                <h4>
                    <span>Detail: </span>
                    <a href=""{_configuration["Setting:UrlFrontEnd"]}/approval/pending-approval"">
                        {_configuration["Setting:UrlFrontEnd"]}/approval/pending-approval                    
                    </a>
                </h4>";

            string bodyMail = $@"";

            userCodeReceiveEmail.Add(request?.EmailCreated ?? "");

            if (request?.CreateLeaveRequestDto.Count > 0) //dữ liệu nhập bằng tay
            {
                foreach (var leave in request.CreateLeaveRequestDto)
                {
                    userCodeReceiveEmail.Add(leave?.UserCode ?? "");

                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        Id = Guid.NewGuid(),
                        ApplicationFormId = newApplicationForm.Id,
                        UserCode = leave?.UserCode,
                        UserName = leave?.UserName,
                    };

                    applicationFormItems.Add(newApplicationFormItem);

                    var newLeaveRequest = new Domain.Entities.LeaveRequest
                    {
                        Id = Guid.NewGuid(),
                        ApplicationFormItemId = newApplicationFormItem.Id,
                        UserCode = leave?.UserCode,
                        UserName = leave?.UserName,
                        DepartmentId = leave?.DepartmentId,
                        Position = leave?.Position,
                        FromDate = leave?.FromDate,
                        ToDate = leave?.ToDate,
                        TypeLeaveId = leave?.TypeLeaveId,
                        TimeLeaveId = leave?.TimeLeaveId,
                        Reason = leave?.Reason,
                        CreatedAt = DateTimeOffset.Now
                    };

                    if (leave?.Image != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await leave.Image.CopyToAsync(memoryStream);
                            var imageData = memoryStream.ToArray();
                            
                            newLeaveRequest.Image = imageData;
                        }
                    }

                    leaveRequests.Add(newLeaveRequest);

                    newLeaveRequest.TypeLeave = typeLeave.FirstOrDefault(e => e.Id == leave?.TypeLeaveId);
                    newLeaveRequest.TimeLeave = timeLeave.FirstOrDefault(e => e.Id == leave?.TimeLeaveId);
                    newLeaveRequest.OrgUnit = orgUnits.FirstOrDefault(e => e.Id == leave?.DepartmentId);

                    bodyMail += TemplateEmail.EmailContentLeaveRequest(newLeaveRequest) + "<br/>";
                }
            }
            else //import bằng excel
            {
                //get all department

                using var workbook = new XLWorkbook(request?.ImportByExcel?.OpenReadStream());
                var worksheet = workbook.Worksheet(1);

                ValidateExcel(worksheet);

                var rows = worksheet?.RangeUsed()?.RowsUsed().Skip(1);


            }


            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.ApplicationForms.Add(newApplicationForm);
                _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
                _context.ApplicationFormItems.AddRange(applicationFormItems);
                _context.LeaveRequests.AddRange(leaveRequests);

                await _context.SaveChangesAsync();

                //gửi email cho người nộp đơn và người xin nghỉ
                List<string> emailSendNoti = (List<string>)await _context.Database.GetDbConnection().QueryAsync<string>($@"
                SELECT COALESCE(NULLIF(U.Email, ''), NV.NVEmail, '') AS Email FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                LEFT JOIN {Global.DbWeb}.dbo.user_configs AS UC ON NV.NVMaNV = UC.UserCode 
                LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
                WHERE NV.NVMaNV IN @UserCodes
                AND (UC.UserCode IS NULL OR (UC.UserCode IS NOT NULL AND UC.[Key] = 'RECEIVE_MAIL_LEAVE_REQUEST' AND UC.Value = 'true'))
                ", new { UserCodes = userCodeReceiveEmail.Distinct().ToList() });

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailRequestHasBeenSent(
                        emailSendNoti.ToList(),
                        null,
                        "Leave request has been submitted.",
                        bodyMail,
                        null,
                        true
                    )
                );

                //gửi email cho hr
                if (status == (int)StatusApplicationFormEnum.WAIT_HR)
                {
                    //send to hr
                    var hr = await GetHrWithManagementLeavePermission();

                    BackgroundJob.Enqueue<IEmailService>(job =>
                        job.SendEmailRequestHasBeenSent(
                            hr.Select(e => e.Email ?? "").ToList(),
                            null,
                            "Request for leave request approval",
                            emailLinkApproval + bodyMail,
                            null,
                            true
                        )
                    );
                }
                else //gửi email cho người duyệt tiếp theo
                {
                    var nextUserOrgPositions = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId);

                    BackgroundJob.Enqueue<IEmailService>(job =>
                        job.SendEmailManyPeopleLeaveRequest(
                            nextUserOrgPositions.Select(e => e.Email ?? "").ToList(),
                            null,
                            "Request for leave request approval",
                            emailLinkApproval + bodyMail,
                            null,
                            true
                        )
                    );
                }

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new ValidationException("Lỗi dữ liệu, kiểm tra lại");
            }
        }

        private async Task<(int nextOrgPositionId, int status)> GetNextOrgPositionAndStatusLeaveRequest(int orgPositionId, string type = "create", int? orgPositionIdApplicationForm = null) //type approval or create
        {
            int nextOrgPositionId = 0;
            int status = (int)StatusApplicationFormEnum.PENDING;

            var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            var approvalFlows = await _context.ApprovalFlows
                .Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPositionId)
                .FirstOrDefaultAsync();

            var orgPosition = await _context.OrgPositions
                .Include(e => e.Unit)
                .FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            if (approvalFlows != null)
            {

            }
            else if (approvalFlows == null)
            {
                //1. là general manager, 2 là trường hợp approval của manager thì gửi đến hr
                if (
                    orgPosition.UnitId == (int)UnitEnum.GM ||
                    (type == "approved" && orgPositionIdApplicationForm != null && orgPositionIdApplicationForm == orgPositionId && orgPosition.UnitId == (int)UnitEnum.Manager)
                )
                {
                    nextOrgPositionId = 0;
                    status = (int)StatusApplicationFormEnum.WAIT_HR;
                }
                else
                {
                    nextOrgPositionId = orgPosition?.ParentOrgPositionId ?? 0;
                    status = (int)StatusApplicationFormEnum.PENDING;
                }
            }

            return (nextOrgPositionId, status);
        }

        private void ValidateExcel(IXLWorksheet worksheet)
        {
            Helper.ValidateExcelHeader(worksheet, ["Mã nhân viên", "Họ tên", "Bộ phận", "Chức vụ", "Loại phép", "Thời gian nghỉ", "Nghỉ từ ngày", "Nghỉ đến ngày", "Lý do"]);


        }

        public Task<Domain.Entities.LeaveRequest> GetById(Guid id)
        {
            throw new NotImplementedException();
        }





        //public Task<object> Approval(ApprovalRequest request)
        //{
        //    throw new NotImplementedException();
        //}



        //public Task<object> Delete(Guid id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<object> GetAll()
        //{
        //    throw new NotImplementedException();
        //}



        //public Task<object> ImportLeaveByExcel()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<object> Update(Guid id, LeaveRequestDto dto)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Lấy danh sách nhưng đơn nghỉ phép của user, nếu như request gửi lên là in process thì hthi những đơn có trạng thái là in process hoặc wait hr
        /// </summary>
        //public async Task<PagedResults<Domain.Entities.LeaveRequest>> GetAll(GetAllLeaveRequest request)
        //{
        //    int pageSize = request.PageSize;
        //    int page = request.Page;
        //    int? status = request.Status;
        //    string? UserCode = request?.UserCode;

        //    var query = _context.LeaveRequests
        //        .Where(l => l.ApplicationForm != null && (l.ApplicationForm.UserCodeRequestor == UserCode || l.ApplicationForm.UserCodeCreated == UserCode) &&
        //                    l.ApplicationForm != null &&
        //                    (
        //                        status == (int)StatusApplicationFormEnum.IN_PROCESS
        //                            ? l.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS || l.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR
        //                            : l.ApplicationForm.RequestStatusId == status
        //                    )
        //        );

        //    var totalItems = await query.CountAsync();
        //    var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        //    var pagedResult = await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .OrderByDescending(x => x.CreatedAt)
        //        .Select(x => new Domain.Entities.LeaveRequest
        //        {
        //            Id = x.Id,
        //            Position = x.Position,
        //            FromDate = x.FromDate,
        //            ToDate = x.ToDate,
        //            Reason = x.Reason,
        //            CreatedAt = x.CreatedAt,
        //            TimeLeave = x.TimeLeave,
        //            TypeLeave = x.TypeLeave,
        //            OrgUnit = x.OrgUnit,
        //            User = x.User,
        //            ApplicationForm = x.ApplicationForm != null ? new ApplicationForm
        //            {
        //                Id = x.ApplicationForm.Id,
        //                Code = x.ApplicationForm.Code,
        //                UserCodeRequestor = x.ApplicationForm.UserCodeRequestor,
        //                UserNameRequestor = x.ApplicationForm.UserNameRequestor,
        //                UserCodeCreated = x.ApplicationForm.UserCodeCreated,
        //                UserNameCreated = x.ApplicationForm.UserNameCreated,
        //                OrgPositionId = x.ApplicationForm.OrgPositionId,
        //                CreatedAt = x.ApplicationForm.CreatedAt,
        //                RequestType = x.ApplicationForm.RequestType,
        //                RequestStatus = x.ApplicationForm.RequestStatus,
        //                HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(h => h.CreatedAt).ToList(),
        //            } : null
        //        })
        //        .ToListAsync();

        //    var countPending = await _context.LeaveRequests
        //        .Include(e => e.ApplicationForm)
        //        .Where(e => e.ApplicationForm != null && (e.ApplicationForm.UserCodeRequestor == UserCode || e.ApplicationForm.UserCodeCreated == UserCode) && e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.PENDING)
        //        .CountAsync();

        //    var countInProcess = await _context.LeaveRequests
        //        .Include(e => e.ApplicationForm)
        //        .Where(e => e.ApplicationForm != null && (e.ApplicationForm.UserCodeRequestor == UserCode || e.ApplicationForm.UserCodeCreated == UserCode) &&
        //            (
        //                e.ApplicationForm.RequestStatusId == 2 || e.ApplicationForm.RequestStatusId == 4
        //            )
        //        )
        //        .CountAsync();

        //    return new PagedResults<Domain.Entities.LeaveRequest>
        //    {
        //        Data = pagedResult,
        //        TotalItems = totalItems,
        //        TotalPages = totalPages,
        //        CountPending = countPending,
        //        CountInProcess = countInProcess,
        //    };
        //}

        //public async Task<Domain.Entities.LeaveRequest> GetById(Guid? id)
        //{
        //    var leaveRequest = await _context.LeaveRequests
        //        .Select(x => new Domain.Entities.LeaveRequest
        //        {
        //            Id = x.Id,
        //            Position = x.Position,
        //            FromDate = x.FromDate,
        //            ToDate = x.ToDate,
        //            Reason = x.Reason,
        //            CreatedAt = x.CreatedAt,
        //            TimeLeave = x.TimeLeave,
        //            TypeLeave = x.TypeLeave,
        //            OrgUnit = x.OrgUnit,
        //            User = x.User,
        //            ApplicationFormId = x.ApplicationFormId,
        //            ApplicationForm = x.ApplicationForm != null ? new ApplicationForm
        //            {
        //                Id = x.ApplicationForm.Id,
        //                Code = x.ApplicationForm.Code,
        //                UserCodeRequestor = x.ApplicationForm.UserCodeRequestor,
        //                UserNameRequestor = x.ApplicationForm.UserNameRequestor,
        //                UserCodeCreated = x.ApplicationForm.UserCodeCreated,
        //                UserNameCreated = x.ApplicationForm.UserNameCreated,
        //                OrgPositionId = x.ApplicationForm.OrgPositionId,
        //                CreatedAt = x.ApplicationForm.CreatedAt,
        //                RequestType = x.ApplicationForm.RequestType,
        //                RequestTypeId = x.ApplicationForm.RequestTypeId,
        //                RequestStatus = x.ApplicationForm.RequestStatus,
        //                HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(h => h.CreatedAt).ToList(),
        //            } : null
        //        })
        //        .FirstOrDefaultAsync(e => e.Id == id || e.ApplicationFormId == id)
        //        ?? throw new NotFoundException("Leave request not found!");

        //    return leaveRequest;
        //}

        //public async Task<object> Update(Guid id, LeaveRequestDto dto)
        //{
        //    var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id || e.ApplicationFormId == id) ?? throw new NotFoundException("Leave request not found!");

        //    leaveRequest.Id = leaveRequest.Id;
        //    leaveRequest.DepartmentId = dto.DepartmentId;
        //    leaveRequest.Position = dto.Position;

        //    leaveRequest.FromDate = dto.FromDate;
        //    leaveRequest.ToDate = dto.ToDate;

        //    leaveRequest.TimeLeaveId = dto.TimeLeaveId;
        //    leaveRequest.TypeLeaveId = dto.TypeLeaveId;
        //    leaveRequest.Reason = dto.Reason;
        //    leaveRequest.UpdateAt = DateTimeOffset.Now;

        //    _context.LeaveRequests.Update(leaveRequest);

        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        //public async Task<object> Delete(Guid id)
        //{
        //    var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id || e.ApplicationFormId == id) ?? throw new NotFoundException("Leave request not found!");

        //    await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == leaveRequest.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

        //    await _context.ApplicationForms.Where(e => e.Id == leaveRequest.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

        //    await _context.LeaveRequests.Where(e => e.Id == leaveRequest.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

        //    return true;
        //}

        ///// <summary>
        ///// Hàm duyệt đơn nghỉ phép, cần orgUnitId, lấy luồng duyệt theo workflowstep nếu approval thì gửi đến người tiếp theo, hoặc k có người tiếp thì gửi đến hr
        ///// khi approval hoặc reject thì sẽ gửi email đến người đó
        //public async Task<object> Approval(ApprovalRequest request)
        //{
        //    var userClaim = _httpContextAccessor.HttpContext.User;

        //    var roleClaims = userClaim.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

        //    var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

        //    var leaveRequest = await GetById((Guid)(request.LeaveRequestId));

        //    var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == leaveRequest.ApplicationFormId) ?? throw new NotFoundException("Application form is not found, please check again");

        //    var historyApplicationForm = new HistoryApplicationForm
        //    {
        //        ApplicationFormId = applicationForm.Id,
        //        CreatedAt = DateTimeOffset.Now
        //    };

        //    var user = await _context.Users.Include(e => e.UserConfigs).FirstOrDefaultAsync(e => e.UserCode == applicationForm.UserCodeRequestor);

        //    int requestStatusApplicationForm = -1;
        //    int? nextOrgPositionId = orgPosition.ParentOrgPositionId;

        //    //bool isComplete = false;
        //    bool isSendHr = false;

        //    //lấy danh sách workflow của người hiện tại, check xem user có custom workflow không
        //    var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

        //    if (approvalFlows != null)
        //    {
        //        if (approvalFlows.IsFinal == true)
        //        {
        //            //send to hr
        //            isSendHr = true;
        //            requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR;
        //            nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
        //        }
        //        else
        //        {
        //            requestStatusApplicationForm = (int)StatusApplicationFormEnum.IN_PROCESS;
        //            nextOrgPositionId = approvalFlows.ToOrgPositionId;
        //        }
        //    }
        //    else if (nextOrgPositionId == null)
        //    {
        //        //send to hr
        //        isSendHr = true;
        //        requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR;
        //        nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
        //    }
        //    else
        //    {
        //        requestStatusApplicationForm = (int)StatusApplicationFormEnum.IN_PROCESS;
        //    }

        //    //case reject
        //    if (request.Status == false)
        //    {

        //        applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
        //        applicationForm.UpdatedAt = DateTimeOffset.Now;
        //        _context.ApplicationForms.Update(applicationForm);

        //        historyApplicationForm.UserNameApproval = request.UserNameApproval;
        //        historyApplicationForm.Action = "REJECT";
        //        historyApplicationForm.Note = request.Note ?? "";
        //        historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

        //        _context.HistoryApplicationForms.Add(historyApplicationForm);

        //        SendRejectEmailLeaveRequest(user, leaveRequest, request.Note ?? "");

        //        await _context.SaveChangesAsync();
        //        return true;
        //    }

        //    applicationForm.Id = applicationForm.Id;
        //    applicationForm.RequestStatusId = requestStatusApplicationForm;
        //    applicationForm.OrgPositionId = nextOrgPositionId;

        //    historyApplicationForm.UserNameApproval = request.UserNameApproval;
        //    historyApplicationForm.Action = "APPROVAL";
        //    historyApplicationForm.Note = request.Note ?? "";
        //    historyApplicationForm.UserCodeApproval = request.UserCodeApproval;

        //    _context.ApplicationForms.Update(applicationForm);
        //    _context.HistoryApplicationForms.Add(historyApplicationForm);

        //    await _context.SaveChangesAsync();

        //    string templateEmail = TemplateEmail.EmailContentLeaveRequest(leaveRequest);

        //    //gửi email thông tin cho người tiếp theo
        //    string urlWaitApproval = $"{_configuration["Setting:UrlFrontEnd"]}/approval/pending-approval";
        //    string emailWithUrlApproval = $@"
        //        <h4>
        //            <span>Detail: </span>
        //            <a href={urlWaitApproval}>{urlWaitApproval}</a>
        //        </h4>" + templateEmail + "<br/>"
        //    ;

        //    if (isSendHr)
        //    {
        //        var hrHavePermissionMngLeaveRequest = await GetHrWithManagementLeavePermission();

        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailRequestHasBeenSent(
        //                hrHavePermissionMngLeaveRequest.Select(e => e.Email ?? "").ToList(),
        //                null,
        //                "Request for leave request approval",
        //                emailWithUrlApproval,
        //                null,
        //                true
        //            )
        //        );
        //    }
        //    else
        //    {
        //        var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailRequestHasBeenSent(
        //                receiveUser.Select(e => e.Email ?? "").ToList(),
        //                null,
        //                "Request for leave request approval",
        //                emailWithUrlApproval,
        //                null,
        //                true
        //            )
        //        );
        //    }

        //    return true;
        //}

        ////hàm send email khi mà approved bước cuối cùng thành công
        //private void SendEmailSuccessLeaveRequest(Domain.Entities.User? userRequester, Domain.Entities.LeaveRequest leaveRequest)
        //{
        //    if (userRequester?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false")
        //    {
        //        return;
        //    }

        //    if (!string.IsNullOrWhiteSpace(userRequester?.Email))
        //    {
        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailRejectLeaveRequest(
        //                new List<string> { userRequester.Email ?? Global.EmailDefault },
        //                null,
        //                "Your leave request has been approved",
        //                TemplateEmail.EmailContentLeaveRequest(leaveRequest),
        //                null,
        //                true
        //            )
        //        );
        //    }
        //}

        ////hàm send email khi mà bị từ chối reject
        //private void SendRejectEmailLeaveRequest(Domain.Entities.User? userRequester, Domain.Entities.LeaveRequest leaveRequest, string rejectionNote)
        //{
        //    if (userRequester?.UserConfigs?.FirstOrDefault(e => e.Key == "RECEIVE_MAIL_LEAVE_REQUEST")?.Value == "false")
        //    {
        //        return;
        //    }

        //    string bodyMailReject = $@"<h4><span style=""color:red"">Reason: {rejectionNote}</span></h4>" +
        //                    TemplateEmail.EmailContentLeaveRequest(leaveRequest) + "<br/>";

        //    if (!string.IsNullOrWhiteSpace(userRequester?.Email))
        //    {
        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailRejectLeaveRequest(
        //                new List<string> { userRequester.Email },
        //                null,
        //                "Your leave request has been rejected",
        //                bodyMailReject,
        //                null,
        //                true
        //            )
        //        );
        //    }
        //}

        ///// <summary>
        ///// cập nhật những người có quyền quản lý nghỉ phép, có thể đăng ký nghỉ phép hộ
        ///// </summary>
        //public async Task<object> UpdateUserHavePermissionCreateMultipleLeaveRequest(List<string> UserCodes)
        //{
        //    var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.create_multiple_leave_request")
        //        ?? throw new NotFoundException("Permission not found");

        //    var userPermissions = await _context.UserPermissions.Where(e => e.PermissionId == permission.Id).ToListAsync();

        //    var currentUserCodes = userPermissions.Select(e => e.UserCode).ToHashSet();
        //    var newUserCodesSet = UserCodes.ToHashSet();
        //    var toRemove = userPermissions.Where(e => !newUserCodesSet.Contains(e?.UserCode ?? "")).ToList();
        //    var toAdd = UserCodes.Where(code => !currentUserCodes.Contains(code)).Select(code => new UserPermission { PermissionId = permission.Id, UserCode = code });

        //    _context.UserPermissions.RemoveRange(toRemove);
        //    _context.UserPermissions.AddRange(toAdd);

        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        ///// <summary>
        ///// lấy những người có quyền quản lý nghỉ phép, có thể đăng ký nghỉ phép hộ
        ///// </summary>
        //public async Task<object> GetUserCodeHavePermissionCreateMultipleLeaveRequest()
        //{
        //    var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.create_multiple_leave_request");

        //    if (permission == null)
        //    {
        //        throw new NotFoundException("Permission not found");
        //    }

        //    return await _context.UserPermissions.Where(e => e.PermissionId == permission.Id).Select(e => e.UserCode).ToListAsync();
        //}

        ///// <summary>
        ///// Chọn những vị trí được quản lý nghỉ phép cho người dùng, vd: người a quản lý tổ A, tổ B
        ///// </summary>
        //public async Task<object> AttachUserManageOrgUnit(AttachUserManageOrgUnitRequest request)
        //{
        //    var userMngOrgUnitIds = await _context.UserMngOrgUnitId
        //        .Where(e => e.UserCode == request.UserCode && e.ManagementType == "MNG_LEAVE_REQUEST")
        //        .ToListAsync();

        //    var existingIds = userMngOrgUnitIds.Select(e => e.OrgUnitId).ToHashSet();
        //    var newIds = request.OrgUnitIds.ToHashSet();

        //    _context.UserMngOrgUnitId.RemoveRange(userMngOrgUnitIds.Where(e => !newIds.Contains((int)(e?.OrgUnitId))));

        //    _context.UserMngOrgUnitId.AddRange(request.OrgUnitIds
        //        .Where(id => !existingIds.Contains(id))
        //        .Select(id => new UserMngOrgUnitId
        //        {
        //            UserCode = request.UserCode,
        //            OrgUnitId = id,
        //            ManagementType = "MNG_LEAVE_REQUEST"
        //        })
        //    );

        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        ///// <summary>
        ///// Lấy những vị trí được quản lý nghỉ phép theo người dùng, vd: ID: 1 (tổ a), ID: 2 (tổ b)
        ///// </summary>
        //public async Task<object> GetOrgUnitIdAttachedByUserCode(string userCode)
        //{
        //    var results = await _context.UserMngOrgUnitId
        //        .Where(e => e.UserCode == userCode && e.ManagementType == "MNG_LEAVE_REQUEST")
        //        .Select(e => e.OrgUnitId)
        //        .ToListAsync();

        //    return results;
        //}

        ///// <summary>
        ///// Tìm kiếm người xin nghỉ phép ở màn tạo nghỉ phép hộ, vd: người a có thể tìm kiếm người a,b,c, k thể tìm kiếm người d, được thiết lập ở màn ql nghỉ phép của HR
        ///// </summary>
        //public async Task<object> SearchUserRegisterLeaveRequest(SearchUserRegisterLeaveRequest request)
        //{
        //    var parameters = new DynamicParameters();

        //    parameters.Add("@UserCodeMng", request.UserCodeRegister, DbType.String, ParameterDirection.Input);
        //    parameters.Add("@Type", "MNG_LEAVE_REQUEST", DbType.String, ParameterDirection.Input);
        //    parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);

        //    var result = await _context.Database.GetDbConnection()
        //        .QueryFirstOrDefaultAsync<object>(
        //            "dbo.SearchUserRegisterLeaveRequest",
        //            parameters,
        //            commandType: CommandType.StoredProcedure
        //    );

        //    if (result != null)
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        throw new ValidationException("Bạn chưa có quyền đăng ký nghỉ phép cho người này, liên hệ HR");
        //    }
        //}

        ///// <summary>
        ///// xin nghỉ phép cho nghiều người khác, gửi cho cấp trên của người tạo đơn nghỉ phép, vd: a viết phép nghỉ cho b, c -> gửi cho cấp trên của a
        ///// </summary>
        //public async Task<object> CreateLeaveForManyPeople(CreateLeaveRequestForManyPeopleRequest request)
        //{
        //    int? orgPositionId = request.OrgPositionId;
        //    string? userCode = request.UserCode;
        //    string? urlFrontEnd = request.UrlFrontEnd;

        //    if (orgPositionId == null)
        //    {
        //        throw new ValidationException(Global.UserNotSetInformation);
        //    }

        //    if (request.Leaves == null || request.Leaves != null && request.Leaves.Count == 0)
        //    {
        //        throw new ValidationException("No one has requested leave, please check again!");
        //    }

        //    var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);
        //    var requesterUser = await _context.Users.Include(e => e.UserConfigs).FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found!");

        //    int requestStatusApplicationForm = -1;
        //    int? nextOrgPositionId = orgPosition.ParentOrgPositionId;
        //    bool isSendHr = false;

        //    var timeLeaves = await _context.TimeLeaves.ToListAsync();
        //    var typeLeaves = await _context.TypeLeaves.ToListAsync();
        //    var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

        //    var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

        //    if (nextOrgPositionId == null || (approvalFlows != null && approvalFlows.IsFinal == true) || nextOrgPositionId == Global.ParentOrgPositionGM) //next org position = 1 là của GM
        //    {
        //        isSendHr = true;
        //        requestStatusApplicationForm = (int)StatusApplicationFormEnum.WAIT_HR; //send hr
        //        nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ; //send hr
        //    }
        //    else if (approvalFlows != null)
        //    {
        //        requestStatusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
        //        nextOrgPositionId = approvalFlows.ToOrgPositionId;
        //    }
        //    else
        //    {
        //        requestStatusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
        //    }

        //    List<ApplicationForm> applicationForms = [];
        //    List<Domain.Entities.LeaveRequest> leaveRequests = [];

        //    List<string> userCodes = [userCode ?? ""];

        //    string urlWaitApproval = $"{_configuration["Setting:UrlFrontEnd"]}/approval/pending-approval";

        //    string emailLinkApproval = $@"
        //        <h4>
        //            <span>Detail: </span>
        //            <a href={urlWaitApproval}>{urlWaitApproval}</a>
        //        </h4>";

        //    string bodyMail = $@"";

        //    foreach (var itemLeave in request.Leaves)
        //    {
        //        userCodes.Add(itemLeave.UserCodeRequestor ?? "");

        //        var newApplicationForm = new ApplicationForm
        //        {
        //            Id = Guid.NewGuid(),
        //            Code = Helper.GenerateFormCode("LR"),
        //            UserCodeRequestor = itemLeave.UserCodeRequestor,
        //            UserNameRequestor = itemLeave.UserNameRequestor,
        //            UserNameCreated = itemLeave.UserNameWriteLeaveRequest,
        //            UserCodeCreated = itemLeave.WriteLeaveUserCode,
        //            DepartmentId = itemLeave.DepartmentId,
        //            RequestTypeId = (int)RequestTypeEnum.LEAVE_REQUEST,
        //            RequestStatusId = requestStatusApplicationForm,
        //            OrgPositionId = nextOrgPositionId,
        //            CreatedAt = DateTimeOffset.Now
        //        };
        //        _context.ApplicationForms.Add(newApplicationForm);

        //        var newLeave = new Domain.Entities.LeaveRequest
        //        {
        //            ApplicationFormId = newApplicationForm.Id,
        //            DepartmentId = itemLeave.DepartmentId,
        //            Position = itemLeave.Position,
        //            FromDate = itemLeave.FromDate,
        //            ToDate = itemLeave.ToDate,
        //            TimeLeaveId = itemLeave.TimeLeaveId,
        //            TypeLeaveId = itemLeave.TypeLeaveId,
        //            Reason = itemLeave.Reason,
        //            CreatedAt = DateTimeOffset.Now
        //        };

        //        _context.LeaveRequests.Add(newLeave);

        //        newLeave.TypeLeave = typeLeaves.FirstOrDefault(e => e.Id == itemLeave.TypeLeaveId);
        //        newLeave.TimeLeave = timeLeaves.FirstOrDefault(e => e.Id == itemLeave.TimeLeaveId);
        //        newLeave.OrgUnit = orgUnits.FirstOrDefault(e => e.Id == itemLeave.DepartmentId);
        //        newLeave.ApplicationForm = newApplicationForm;

        //        bodyMail += TemplateEmail.EmailContentLeaveRequest(newLeave) + "<br/>";
        //    }

        //    await _context.SaveChangesAsync();

        //    List<string> emailSendNoti = (List<string>)await _context.Database.GetDbConnection().QueryAsync<string>($@"
        //            SELECT COALESCE(NULLIF(U.Email, ''), NV.NVEmail, '') AS Email FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
        //            LEFT JOIN {Global.DbWeb}.dbo.user_configs AS UC ON NV.NVMaNV = UC.UserCode 
        //            LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
        //            WHERE NV.NVMaNV IN @UserCodes
        //            AND (UC.UserCode IS NULL OR (UC.UserCode IS NOT NULL AND UC.[Key] = 'RECEIVE_MAIL_LEAVE_REQUEST' AND UC.Value = 'true'))
        //            ", new { UserCodes = userCodes.Distinct().ToList() });

        //    BackgroundJob.Enqueue<IEmailService>(job =>
        //        job.SendEmailRequestHasBeenSent(
        //            emailSendNoti.ToList(),
        //            null,
        //            "Leave request has been submitted.",
        //            bodyMail,
        //            null,
        //            true
        //        )
        //    );

        //    //gửi email cho hr
        //    if (isSendHr)
        //    {
        //        var hrHavePermissionMngLeaveRequest = await GetHrWithManagementLeavePermission();

        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailRequestHasBeenSent(
        //                hrHavePermissionMngLeaveRequest.Select(e => e.Email ?? "").ToList(),
        //                null,
        //                "Request for leave request approval",
        //                emailLinkApproval + bodyMail,
        //                null,
        //                true
        //            )
        //        );
        //    }
        //    else //gửi email cho người duyệt tiếp theo
        //    {
        //        var userNextOrgPosition = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

        //        BackgroundJob.Enqueue<IEmailService>(job =>
        //            job.SendEmailManyPeopleLeaveRequest(
        //                userNextOrgPosition.Select(e => e.Email ?? "").ToList(),
        //                null,
        //                "Request for leave request approval",
        //                emailLinkApproval + bodyMail,
        //                null,
        //                true
        //            )
        //        );
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// Hàm HR đăng ký nghỉ phép tất cả, lấy những request có trạng tháo là wait hr và vị trí của HR mặc định là -10
        ///// </summary>
        //public async Task<object> HrRegisterAllLeave(HrRegisterAllLeaveRequest request)
        //{
        //    var parsedIds = request.LeaveRequestIds
        //        .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
        //        .Where(g => g != null)
        //        .Select(g => g.Value)
        //        .ToList();

        //    var leaveRequestsWaitHrApproval = await _context.LeaveRequests
        //        .AsNoTrackingWithIdentityResolution()
        //        .Include(e => e.OrgUnit)
        //        .Include(e => e.TimeLeave)
        //        .Include(e => e.TypeLeave)
        //        .Include(e => e.ApplicationForm)
        //        .Include(e => e.User)
        //            .ThenInclude(e => e.UserConfigs)
        //        .Where(e =>
        //            e.ApplicationForm != null &&
        //            e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR &&
        //            e.ApplicationForm.OrgPositionId == (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ &&
        //            (parsedIds.Contains(e.Id) || parsedIds.Contains(e.ApplicationFormId ?? Guid.Empty))
        //        )
        //        .ToListAsync();

        //    var userCodeRequestors = leaveRequestsWaitHrApproval.Select(e => e?.ApplicationForm?.UserCodeRequestor).Distinct().ToList();

        //    var users = await _context.Users.Where(e => userCodeRequestors.Contains(e.UserCode)).Include(e => e.UserConfigs).ToListAsync();

        //    foreach (var itemLeave in leaveRequestsWaitHrApproval)
        //    {
        //        var applicationForm = itemLeave.ApplicationForm;

        //        if (applicationForm != null)
        //        {
        //            applicationForm.Id = applicationForm.Id;
        //            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
        //            applicationForm.OrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;

        //            _context.ApplicationForms.Update(applicationForm);

        //            var historyApplicationForm = new HistoryApplicationForm
        //            {
        //                ApplicationFormId = applicationForm.Id,
        //                UserNameApproval = request.UserName,
        //                Action = "APPROVAL",
        //                UserCodeApproval = request.UserCode,
        //                CreatedAt = DateTimeOffset.Now
        //            };

        //            _context.HistoryApplicationForms.Add(historyApplicationForm);

        //            SendEmailSuccessLeaveRequest(users.FirstOrDefault(e => e.UserCode == applicationForm.UserCodeRequestor), itemLeave);
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return true;
        //}

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
                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
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

        ///// <summary>
        ///// Thêm quyền hr quản lý nghỉ phép
        ///// </summary>
        //public async Task<object> UpdateHrWithManagementLeavePermission(List<string> UserCode)
        //{
        //    var permissionHrMngLeaveRequest = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

        //    if (permissionHrMngLeaveRequest == null)
        //    {
        //        throw new NotFoundException("Permission hr manage leave request not found");
        //    }

        //    var oldUserPermissionsMngTKeeping = await _context.UserPermissions.Where(e => e.PermissionId == permissionHrMngLeaveRequest.Id).ToListAsync();

        //    _context.UserPermissions.RemoveRange(oldUserPermissionsMngTKeeping);

        //    List<UserPermission> newUserPermissions = new List<UserPermission>();

        //    foreach (var code in UserCode)
        //    {
        //        newUserPermissions.Add(new UserPermission
        //        {
        //            UserCode = code,
        //            PermissionId = permissionHrMngLeaveRequest.Id
        //        });
        //    }

        //    _context.UserPermissions.AddRange(newUserPermissions);

        //    await _context.SaveChangesAsync();

        //    return true;
        //}

        ///// <summary>
        ///// Hàm HR export leave request
        ///// </summary>
        //public async Task<byte[]> HrExportExcelLeaveRequest(List<string> leaveRequestIds)
        //{
        //    var parsedIds = leaveRequestIds
        //        .Select(id => Guid.TryParse(id, out var guid) ? guid : (Guid?)null)
        //        .Where(guid => guid.HasValue)
        //        .Select(guid => guid.Value)
        //        .ToList();

        //    var leaveRequestsWaitHrApproval = await _context.LeaveRequests
        //        .Include(e => e.ApplicationForm)
        //        .Include(e => e.TimeLeave)
        //        .Include(e => e.TypeLeave)
        //        .Where(e =>
        //            e.ApplicationForm != null &&
        //            e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.WAIT_HR &&
        //            e.ApplicationForm.OrgPositionId == (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ &&
        //            (parsedIds.Contains(e.Id) || parsedIds.Contains(e.ApplicationFormId ?? Guid.Empty))
        //        )
        //        .ToListAsync();

        //    return _excelService.ExportLeaveRequestToExcel(leaveRequestsWaitHrApproval);
        //}

        //public async Task<LeaveRequestStatisticalResponse> StatisticalLeaveRequest(int year)
        //{
        //    using var connection = _context.Database.GetDbConnection();
        //    if (connection.State != ConnectionState.Open)
        //    {
        //        await connection.OpenAsync();
        //    }

        //    using var multi = await connection.QueryMultipleAsync("GetLeaveRequestStatisticalData", new { Year = year }, commandType: CommandType.StoredProcedure);

        //    var result = new LeaveRequestStatisticalResponse
        //    {
        //        GroupByTotal = await multi.ReadFirstAsync<GroupByTotal>(),
        //        GroupRecentList = (await multi.ReadAsync<GroupRecentList>()).ToList(),
        //        GroupByDepartment = (await multi.ReadAsync<GroupByDepartment>()).ToList(),
        //        GroupByMonth = (await multi.ReadAsync<GroupByMonth>()).ToList()
        //    };

        //    return result;
        //}
    }
}
