using System.Data;
using System.Security.Claims;
using ClosedXML.Excel;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Extensions;
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
            int orgPositionId = request.OrgPositionId;

            if (orgPositionId <= 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            List<ApplicationFormItem> applicationFormItems = [];
            List<Domain.Entities.LeaveRequest> leaveRequests = [];
            List<string> userCodeReceiveEmail = [request.EmailCreated];

            var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            var approvalFlows = await _context.ApprovalFlows
                .Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPositionId)
                .FirstOrDefaultAsync();

            var orgPosition = await _context.OrgPositions
                .FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            int statusId = 9999;
            int nextOrgPositionId = 9999;
            bool isSendHr = false;

            if (approvalFlows != null)
            {
                statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                nextOrgPositionId = approvalFlows.ToOrgPositionId ?? 9999;
            }
            else
            {
                if (orgPosition.UnitId == (int)UnitEnum.GM)
                {
                    isSendHr = true;
                    statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                    nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
                }
                else
                {
                    statusId = (int)StatusApplicationFormEnum.PENDING;
                    nextOrgPositionId = orgPosition.ParentOrgPositionId ?? 9999;
                }
            }

            var newApplicationForm = new ApplicationForm
            {
                Code = Helper.GenerateFormCode("LR"),
                RequestTypeId = (int)RequestTypeEnum.LEAVE_REQUEST,
                RequestStatusId = statusId,
                OrgPositionId = nextOrgPositionId,
                UserCodeCreatedForm = request?.UserCodeCreated,
                UserNameCreatedForm = request?.UserNameCreated,
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

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (request?.CreateListLeaveRequests?.Count > 0) //dữ liệu nhập bằng tay
                {
                    foreach (var leave in request.CreateListLeaveRequests)
                    {
                        if (!string.IsNullOrWhiteSpace(leave.UserCode))
                        {
                            userCodeReceiveEmail.Add(leave.UserCode);
                        }

                        var newApplicationFormItem = new ApplicationFormItem
                        {
                            ApplicationForm = newApplicationForm,
                            UserCode = leave.UserCode,
                            UserName = leave.UserName,
                            Status = true,
                            CreatedAt = DateTimeOffset.Now
                        };
                        applicationFormItems.Add(newApplicationFormItem);

                        var newLeaveRequest = new Domain.Entities.LeaveRequest
                        {
                            ApplicationFormItem = newApplicationFormItem,
                            UserCode = leave.UserCode,
                            UserName = leave.UserName,
                            DepartmentId = leave.DepartmentId,
                            Position = leave.Position,
                            FromDate = leave.FromDate,
                            ToDate = leave.ToDate,
                            TimeLeaveId = leave.TimeLeaveId,
                            TypeLeaveId = leave.TypeLeaveId,
                            Reason = leave.Reason,
                            CreatedAt = DateTimeOffset.Now
                        };
                        leaveRequests.Add(newLeaveRequest);
                    }

                    _context.ApplicationForms.Add(newApplicationForm);
                    _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
                    _context.ApplicationFormItems.AddRange(applicationFormItems);
                    _context.LeaveRequests.AddRange(leaveRequests);

                    await _context.SaveChangesAsync();

                    foreach (var (leave, leaveRequest) in request.CreateListLeaveRequests.Zip(leaveRequests))
                    {
                        if (leave?.Image != null)
                        {
                            using var memoryStream = new MemoryStream();
                            await leave.Image.CopyToAsync(memoryStream);

                            var fileEntity = new Domain.Entities.File
                            {
                                FileName = leave.Image.FileName,
                                ContentType = leave.Image.ContentType,
                                FileData = memoryStream.ToArray(),
                                CreatedAt = DateTimeOffset.Now
                            };
                            _context.Files.Add(fileEntity);

                            var attachment = new AttachFile
                            {
                                File = fileEntity,
                                EntityType = nameof(Domain.Entities.LeaveRequest),
                                EntityId = leaveRequest.Id
                            };
                            _context.AttachFiles.Add(attachment);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                else //import by excel
                {
                    var orgUnitDepartments = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();
                    using var workbook = new XLWorkbook(request?.File?.OpenReadStream());
                    var worksheet = workbook.Worksheet(1);

                    var resultApplicationFormItemAndLeaveRequets = await ValidateExcel(worksheet, newApplicationForm);

                    applicationFormItems = resultApplicationFormItemAndLeaveRequets.applicationFormItems;
                    leaveRequests = resultApplicationFormItemAndLeaveRequets.leaveRequests;

                    userCodeReceiveEmail = userCodeReceiveEmail
                        .Concat(
                            leaveRequests
                                .Where(l => !string.IsNullOrWhiteSpace(l.UserCode))
                                .Select(l => l.UserCode!)
                        )
                        .Distinct()
                        .ToList();

                    _context.ApplicationForms.Add(newApplicationForm);
                    _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
                    _context.ApplicationFormItems.AddRange(applicationFormItems);
                    _context.LeaveRequests.AddRange(leaveRequests);

                    await _context.SaveChangesAsync();

                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            //gửi email cho người nộp đơn và người xin nghỉ
            List<string> emailSendNoti = (List<string>)await _context.Database.GetDbConnection().QueryAsync<string>($@"
                SELECT COALESCE(NULLIF(U.Email, ''), NV.NVEmail, '') AS Email FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                LEFT JOIN {Global.DbWeb}.dbo.user_configs AS UC ON NV.NVMaNV = UC.UserCode 
                LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
                WHERE NV.NVMaNV IN @UserCodes
                AND (UC.UserCode IS NULL OR (UC.UserCode IS NOT NULL AND UC.[Key] = 'RECEIVE_MAIL_LEAVE_REQUEST' AND UC.Value = 'true'))
                ", new { UserCodes = userCodeReceiveEmail.Distinct().ToList() });

            string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/leave-request/{newApplicationForm.Code}";
            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/view-leave-request-approval/{newApplicationForm.Code}";

            //gửi email cho người nộp đơn và người xin nghỉ
            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailLeaveRequest(
                    emailSendNoti.ToList(),
                    null,
                    "Leave request has been submitted.",
                    TemplateEmail.SendContentEmail("Leave request has been submitted.", urlView, newApplicationForm.Code),
                    null,
                    true
                )
            );

            List<GetMultiUserViClockByOrgPositionIdResponse> nextUserApprovals = [];

            //hr
            if (isSendHr)
            {
                var permissionHrMngLeaveRequest = await _context.Permissions
                    .Include(e => e.UserPermissions)
                    .FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

                nextUserApprovals = await _userService.GetMultipleUserViclockByOrgPositionId(-1, permissionHrMngLeaveRequest?.UserPermissions?.Select(e => e.UserCode ?? "")?.ToList());
            }
            else //gửi email cho người duyệt tiếp theo
            {
                nextUserApprovals = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId);
            }

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailLeaveRequest(
                    nextUserApprovals.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for leave request approval",
                    TemplateEmail.SendContentEmail("Request for leave request approval", urlApproval, newApplicationForm.Code),
                    null,
                    true
                )
            );

            return true;
        }

        private async Task<(List<ApplicationFormItem> applicationFormItems, List<Domain.Entities.LeaveRequest> leaveRequests)> ValidateExcel(
            IXLWorksheet worksheet, 
            ApplicationForm newApplicationForm
        )
        {
            List<string> checkDepartments = [];
            List<string> checkUserCodesCanLeaveRq = [];

            List<ApplicationFormItem> applicationFormItems = [];
            List<Domain.Entities.LeaveRequest> leaveRequests = [];

            Helper.ValidateExcelHeader(worksheet, ["Mã nhân viên", "Họ tên", "Bộ phận", "Chức vụ", "Loại phép", "Thời gian nghỉ", "Nghỉ từ ngày", "Nghỉ đến ngày", "Lý do"]);

            var rows = (worksheet?.RangeUsed()?.RowsUsed().Skip(2)) ?? throw new ValidationException("Không có dữ liệu nào, kiểm tra lại file excel");

            var orgUnitDepartments = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();
            var timeLeaves = await _context.TimeLeaves.ToListAsync();
            var typeLeaves = await _context.TypeLeaves.ToListAsync();

            int currentRow = 3;

            foreach (var row in rows)
            {
                string userCode = row.Cell(1).GetValue<string>();
                string userName = row.Cell(2).GetValue<string>();
                string department = row.Cell(3).GetValue<string>();
                string position = row.Cell(4).GetValue<string>();
                string typeLeave = row.Cell(5).GetValue<string>();
                string timeLeave = row.Cell(6).GetValue<string>();
                string strFromDate = row.Cell(7).GetValue<string>();
                string strToDate = row.Cell(7).GetValue<string>();
                DateTimeOffset fromDate;
                DateTimeOffset toDate;
                string reason = row.Cell(9).GetValue<string>();

                bool isEmptyRow = string.IsNullOrWhiteSpace(userCode)
                    && string.IsNullOrWhiteSpace(userName)
                    && string.IsNullOrWhiteSpace(department)
                    && string.IsNullOrWhiteSpace(position)
                    && string.IsNullOrWhiteSpace(typeLeave)
                    && string.IsNullOrWhiteSpace(timeLeave)
                    && string.IsNullOrWhiteSpace(strFromDate)
                    && string.IsNullOrWhiteSpace(strToDate)
                    && string.IsNullOrWhiteSpace(reason);

                if (isEmptyRow)
                {
                    break;
                }

                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(userCode))
                    errors.Add("Mã nhân viên không được để trống");

                if (string.IsNullOrWhiteSpace(userName))
                    errors.Add("Họ tên không được để trống");

                if (string.IsNullOrWhiteSpace(department))
                    errors.Add("Phòng ban không được để trống");

                if (string.IsNullOrWhiteSpace(position))
                    errors.Add("Chức vụ không được để trống");

                if (string.IsNullOrWhiteSpace(typeLeave))
                    errors.Add("Loại nghỉ phép không được để trống");

                if (string.IsNullOrWhiteSpace(timeLeave))
                    errors.Add("Số giờ/ngày nghỉ không được để trống");

                if (!DateTimeOffset.TryParse(row.Cell(7).GetValue<string>(), out fromDate))
                    errors.Add("Ngày bắt đầu không hợp lệ");

                if (!DateTimeOffset.TryParse(row.Cell(8).GetValue<string>(), out toDate))
                    errors.Add("Ngày kết thúc không hợp lệ");

                if (string.IsNullOrWhiteSpace(reason))
                    errors.Add("Lý do không được để trống");

                if (errors.Any())
                {
                    throw new ValidationException($"Lỗi ở dòng {currentRow}: {string.Join("; ", errors)}");
                }

                checkUserCodesCanLeaveRq.Add(userCode);

                int departmentId = orgUnitDepartments?.FirstOrDefault(e => e.Name == department)?.Id
                    ?? throw new ValidationException($"Lỗi ở dòng {currentRow}, phòng ban không chính xác");

                int timeLeaveId = timeLeaves?.FirstOrDefault(e => e.Id == (timeLeave == "CN" ? 1 : timeLeave == "S" ? 2 : 3))?.Id
                    ?? throw new ValidationException($"Lỗi ở dòng {currentRow}, thời gian nghỉ không chính xác");

                int typeLeaveId = typeLeaves?.FirstOrDefault(e => e.Code?.ToLower() == typeLeave.ToLower())?.Id
                    ?? throw new ValidationException($"Lỗi ở dòng {currentRow}, loại nghỉ phép không chính xác");

                var newApplicationFormItem = new ApplicationFormItem
                {
                    ApplicationForm = newApplicationForm,
                    UserCode = userCode,
                    UserName = userName,
                    Status = true,
                    CreatedAt = DateTimeOffset.Now
                };

                applicationFormItems.Add(newApplicationFormItem);

                var newLeaveRequest = new Domain.Entities.LeaveRequest
                {
                    ApplicationFormItem = newApplicationFormItem,
                    UserCode = userCode,
                    UserName = userName,
                    DepartmentId = departmentId,
                    Position = position,
                    FromDate = fromDate,
                    ToDate = toDate,
                    TypeLeaveId = typeLeaveId,
                    TimeLeaveId = timeLeaveId,
                    Reason = reason,
                    CreatedAt = DateTimeOffset.Now
                };

                leaveRequests.Add(newLeaveRequest);
                currentRow++;
            }

            //check xem co ton tai khong
            //checkUserCodesCanLeaveRq
            return (applicationFormItems, leaveRequests);
        }

        public async Task<PagedResults<MyLeaveRequestResponse>> GetMyLeaveRequest(MyLeaveRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@Status", request.Status, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<MyLeaveRequestResponse>(
                    "dbo.Leave_GET_GetMyLeave",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<MyLeaveRequestResponse>
            {
                Data = (List<MyLeaveRequestResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResults<MyLeaveRequestRegisteredResponse>> GetMyLeaveRequestRegistered(MyLeaveRequestRegistered request)
        {
            var query = _context.ApplicationForms
                .Where(e => 
                    e.UserCodeCreatedForm == request.UserCode && 
                    e.DeletedAt == null && 
                    e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST
                );

            if (request.Status != null)
            {
                query = query.Where(e => e.RequestStatusId == request.Status);
            }

            var totalItem = await query.CountAsync();

            var results = await query
                .OrderByDescending(e => e.CreatedAt)
                .Include(e => e.RequestStatus)
                .Include(e => e.RequestType)
                .Select(x => new MyLeaveRequestRegisteredResponse
                {
                    Id = x.Id,
                    Code = x.Code,
                    UserNameCreatedForm = x.UserNameCreatedForm,
                    CreatedAt = x.CreatedAt,
                    RequestStatus = x.RequestStatus,
                    RequestType = x.RequestType
                })
                .Skip((int)((request.Page - 1) * request.PageSize)).Take((int)request.PageSize)
                .ToListAsync();

            int totalPages = (int)Math.Ceiling((double)totalItem / request.PageSize);

            return new PagedResults<MyLeaveRequestRegisteredResponse>
            {
                Data = results,
                TotalItems = totalItem,
                TotalPages = totalPages
            };
        }

        public async Task<object> Delete(string applicationFormCode)
        {
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Code == applicationFormCode)
                ?? throw new NotFoundException("Application form not found, please check again");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTimeOffset.Now;

                await _context.LeaveRequests
                .Where(e => e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationForm != null && e.ApplicationFormItem.ApplicationForm.Id == applicationForm.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(af => af.DeletedAt, now));

                await _context.HistoryApplicationForms
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(h => h.DeletedAt, now));

                await _context.ApplicationFormItems
                    .Where(afi => afi.ApplicationFormId == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, now));

                await _context.ApplicationForms
                    .Where(af => af.Id == applicationForm.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(af => af.DeletedAt, now));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }

            return true;
        }

        public async Task<object> GetLeaveByAppliationFormCode(string applicationFormCode)
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(e => e.ApplicationFormItem)
                .Include(e => e.TypeLeave)
                .Include(e => e.TimeLeave)
                .Include(e => e.OrgUnit)
                .Where(e =>
                    e.ApplicationFormItem != null &&
                    e.ApplicationFormItem.ApplicationForm != null && 
                    e.ApplicationFormItem.ApplicationForm.Code == applicationFormCode
                )
                .OrderByDescending(e => e!.ApplicationFormItem!.Status)
                .ToListAsync();

            foreach (var itemLeave in leaveRequests)
            {
                itemLeave!.ApplicationFormItem!.LeaveRequests = [];
            }

            var applicationForm = await _context.ApplicationForms.LoadApplicationForm(_context, applicationFormCode);

            if (applicationForm != null)
            {
                applicationForm.HistoryApplicationForms = await _context.Set<HistoryApplicationForm>()
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .OrderByDescending(e => e.ActionAt)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return new
            {
                leaveRequests,
                applicationForm
            };
        }

        public async Task<object> Update(string applicationFormCode, List<CreateListLeaveRequest> listLeaveRequests)
        {
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Code == applicationFormCode);

            if (applicationForm == null)
            {
                throw new NotFoundException("Application form not found, please check again");
            }

            //delete
            var leaveDeletes = await _context.LeaveRequests
                .Where(e => 
                    e.ApplicationFormItem != null && 
                    e.ApplicationFormItem.ApplicationForm != null &&
                    e.ApplicationFormItem.ApplicationForm.Code == applicationFormCode && 
                    !listLeaveRequests.Select(e => e.Id).Contains(e.Id)
                )
                .ToListAsync();

            await _context.ApplicationFormItems
                .Where(afi => leaveDeletes.Select(l => l.ApplicationFormItemId).Contains(afi.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, DateTimeOffset.Now));

            await _context.LeaveRequests
                .Where(e => leaveDeletes.Select(l => l.Id).Contains(e.Id))
                .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, DateTimeOffset.Now));

            //update
            var existingLeaves = await _context.LeaveRequests.Where(l => listLeaveRequests.Select(u => u.Id).Contains(l.Id)).ToListAsync();

            foreach (var itemUpdateLeave in listLeaveRequests)
            {
                var leave = existingLeaves.FirstOrDefault(x => x.Id == itemUpdateLeave.Id);

                if (leave != null)
                {
                    leave.Position = itemUpdateLeave.Position;
                    leave.FromDate = itemUpdateLeave.FromDate;
                    leave.ToDate = itemUpdateLeave.ToDate;
                    leave.TypeLeaveId = itemUpdateLeave.TypeLeaveId;
                    leave.TimeLeaveId = itemUpdateLeave.TimeLeaveId;
                    leave.Reason = itemUpdateLeave.Reason;
                    leave.UpdateAt = DateTimeOffset.Now;

                    _context.LeaveRequests.Update(leave);
                }
                else //add new
                {
                    var newApplicationFormItem = new ApplicationFormItem
                    {
                        ApplicationFormId = applicationForm.Id,
                        UserCode = itemUpdateLeave?.UserCode,
                        UserName = itemUpdateLeave?.UserName,
                        Status = true,
                        CreatedAt = DateTimeOffset.Now
                    };

                    var newLeave = new Domain.Entities.LeaveRequest
                    {
                        ApplicationFormItem = newApplicationFormItem,
                        UserCode = itemUpdateLeave?.UserCode,
                        UserName = itemUpdateLeave?.UserName,
                        DepartmentId = itemUpdateLeave!.DepartmentId,
                        Position = itemUpdateLeave?.Position,
                        FromDate = itemUpdateLeave?.FromDate,
                        ToDate = itemUpdateLeave?.ToDate,
                        TypeLeaveId = itemUpdateLeave!.TypeLeaveId,
                        TimeLeaveId = itemUpdateLeave!.TimeLeaveId,
                        Reason = itemUpdateLeave?.Reason,
                        CreatedAt = DateTimeOffset.Now
                    };
                    _context.ApplicationFormItems.Add(newApplicationFormItem);
                    _context.LeaveRequests.Add(newLeave);
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

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

        public async Task<object> RejectSomeLeaves(RejectSomeLeavesRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Code == request.ApplicationFormCode)
                ?? throw new NotFoundException("Application form not found, please check again");

                var todayDate = DateTimeOffset.Now.Date;

                //delegation
                var isDelegation = await _context.Delegations
                    .AnyAsync(e =>
                        e.FromOrgPositionId == applicationForm.OrgPositionId &&
                        e.IsActive == true &&
                        e.UserCodeDelegation == request.UserCodeReject &&
                        todayDate >= e.StartDate!.Value.Date && todayDate <= e.EndDate!.Value.Date
                    );

                if (applicationForm.OrgPositionId != request.OrgPositionId && isDelegation == false)
                {
                    throw new ValidationException(Global.NotPermissionApproval);
                }

                var leaves = await _context.LeaveRequests
                    .Where(e => request.LeaveIds.Contains(e.Id))
                    .ToListAsync();

                await _context.ApplicationFormItems
                    .Where(e => leaves.Select(l => l.ApplicationFormItemId).Contains(e.Id))
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(afi => afi.RejectedAt, DateTimeOffset.Now)
                        .SetProperty(afi => afi.Note, request.Note)
                        .SetProperty(afi => afi.Status, false)
                    );

                var newHistoryApplicationForm = new HistoryApplicationForm
                {
                    ApplicationFormId = applicationForm.Id,
                    Note = $@"{request.Note} __Reject: {string.Join(", ", leaves.Select(l => l.UserName))}",
                    Action = "Reject",
                    UserCodeAction = request.UserCodeReject,
                    UserNameAction = request.UserNameReject,
                    ActionAt = DateTimeOffset.Now
                };

                _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/leave-request/{applicationForm.Code}";
                var userReceivedEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, leaves.Select(e => e.UserCode ?? "").ToList());

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailLeaveRequest(
                        userReceivedEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your leave request has been rejected",
                        TemplateEmail.SendContentEmail("Your leave request has been rejected", urlView, applicationForm.Code ?? ""),
                        null,
                        true
                    )
                );

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Save failed, please check again");
            }
        }

        /// <summary>
        /// Hàm duyệt đơn nghỉ phép, cần orgUnitId, lấy luồng duyệt theo workflowstep nếu approval thì gửi đến người tiếp theo, hoặc k có người tiếp thì gửi đến hr
        /// khi approval hoặc reject thì sẽ gửi email đến người đó
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
                .FirstOrDefaultAsync(e => e.Id == (isDelegation ? applicationForm.OrgPositionId : request.OrgPositionId ))
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

            List<string> UserCodeCreatedFormAndLeave = [applicationForm.UserCodeCreatedForm];
            UserCodeCreatedFormAndLeave.AddRange(applicationForm.ApplicationFormItems.Where(e => e.Status == true).Select(e => e.UserCode ?? "").ToList());
            UserCodeCreatedFormAndLeave = UserCodeCreatedFormAndLeave.Distinct().ToList();

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

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/leave-request/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, UserCodeCreatedFormAndLeave);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailLeaveRequest(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your leave request has been approved",
                        TemplateEmail.SendContentEmail("Your leave request has been approved", urlView, applicationForm.Code ?? ""),
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

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/leave-request/{applicationForm.Code}";

                var userReceiveEmail = await _userService.GetMultipleUserViclockByOrgPositionId(-1, UserCodeCreatedFormAndLeave);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailLeaveRequest(
                        userReceiveEmail.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your leave request has been rejected",
                        TemplateEmail.SendContentEmail("Your leave request has been rejected", urlView, applicationForm.Code ?? ""),
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
            var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPosition.Id).FirstOrDefaultAsync();

            if (approvalFlows != null)
            {
                statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                nextOrgPositionId = approvalFlows.ToOrgPositionId ?? 9999;
            }
            else
            {
                if (orgPosition.UnitId == (int)UnitEnum.GM)
                {
                    isSendHr = true;
                    statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                    nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
                }
                else if (orgPosition.UnitId == (int)UnitEnum.Manager)
                {
                    if (applicationForm.UserCodeCreatedForm != request.UserCodeApproval)
                    {
                        isSendHr = true;
                        statusId = (int)StatusApplicationFormEnum.WAIT_HR;
                        nextOrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_HR_LEAVE_RQ;
                    }
                    else
                    {
                        statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                        nextOrgPositionId = orgPosition.ParentOrgPositionId ?? 9999;
                    }
                }
                else
                {
                    statusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                    nextOrgPositionId = orgPosition.ParentOrgPositionId ?? 9999;
                }
            }

            applicationForm.RequestStatusId = statusId;
            applicationForm.OrgPositionId = nextOrgPositionId;

            historyApplicationForm.Action = "Approved";

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/view-leave-request-approval/{applicationForm.Code}";

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
                job.SendEmailLeaveRequest(
                    nextUserApproval.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for leave request approval",
                    TemplateEmail.SendContentEmail("Request for leave request approval", urlApproval, applicationForm.Code ?? ""),
                    null,
                    true
                )
            );

            return true;
        }

        /// <summary>
        /// Hàm HR export leave request
        /// </summary>
        public async Task<byte[]> HrExportExcelLeaveRequest(long applicationFormId)
        {
            var leaveDatas = await _context.LeaveRequests
                .Include(e => e.TimeLeave)
                .Include(e => e.TypeLeave)
                .Where(e => 
                    e.ApplicationFormItem != null && e.ApplicationFormItem.Status == true &&
                    e.ApplicationFormItem.ApplicationForm != null && e.ApplicationFormItem.ApplicationForm.Id == applicationFormId
                )
                .ToListAsync();

            return _excelService.ExportLeaveRequestToExcel(leaveDatas);
        }
        
        public async Task<object> HrNote(HrNoteRequest request)
        {
            var leave = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == request.LeaveRequestId);
            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == request.ApplicationFormId);

            if (leave == null || applicationForm == null)
            {
                throw new NotFoundException("Leave request not found, please check again");
            }

            await _context.LeaveRequests.Where(e => e.Id == request.LeaveRequestId).ExecuteUpdateAsync(s => s.SetProperty(h => h.NoteOfHR, request.NoteOfHr));

            string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/view/leave-request/{applicationForm.Code}";

            List<string> userCodeReceiveMail = [request.UserCode, leave.UserCode, applicationForm.UserCodeCreatedForm];

            var dataUserEmails = await _userService.GetMultipleUserViclockByOrgPositionId(-1, userCodeReceiveMail.Distinct().ToList());

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailLeaveRequest(
                    dataUserEmails.Where(e => e.NVMaNV != request.UserCode).Select(e => e.Email ?? "").ToList(), //to email
                    dataUserEmails.Where(e => e.NVMaNV == request.UserCode).Select(e => e.Email ?? "").ToList(), //cc email
                    "HR Note",
                    TemplateEmail.SendContentEmail(request.NoteOfHr ?? "Note of HR", urlView, applicationForm.Code ?? ""),
                    null,
                    true
                )
            );

            return true;
        }

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

        /// <summary>
        /// Lấy những vị trí được quản lý nghỉ phép theo người dùng, vd: ID: 1 (tổ a), ID: 2 (tổ b)
        /// </summary>
        //public async Task<object> GetOrgUnitIdAttachedByUserCode(string userCode)
        //{
        //    var results = await _context.UserMngOrgUnitId
        //        .Where(e => e.UserCode == userCode && e.ManagementType == "MNG_LEAVE_REQUEST")
        //        .Select(e => e.OrgUnitId)
        //        .ToListAsync();

        //    return results;
        //}

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
    }
}
