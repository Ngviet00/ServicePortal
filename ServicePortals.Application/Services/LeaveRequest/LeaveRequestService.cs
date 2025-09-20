using System.Data;
using System.Dynamic;
using ClosedXML.Excel;
using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.LeaveRequest.Requests;
using ServicePortals.Application.Dtos.LeaveRequest.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            return null;
            //int orgPositionId = request?.OrgPositionId ?? 0;

            //if (orgPositionId <= 0)
            //{
            //    throw new ValidationException(Global.UserNotSetInformation);
            //}

            //List<ApplicationFormItem> applicationFormItems = [];
            //List<Domain.Entities.LeaveRequest> leaveRequests = [];
            //List<string> userCodeReceiveEmail = [];

            //var nextOrgPositionAndStatus = await GetNextOrgPositionAndStatusLeaveRequest(orgPositionId);

            //int nextOrgPositionId = nextOrgPositionAndStatus.nextOrgPositionId;
            //int status = nextOrgPositionAndStatus.status;

            //var newApplicationForm = new ApplicationForm
            //{
            //    Id = Guid.NewGuid(),
            //    Code = Helper.GenerateFormCode("LR"),
            //    RequestTypeId = (int)RequestTypeEnum.LEAVE_REQUEST,
            //    RequestStatusId = status,
            //    UserCodeCreatedBy = request?.UserCodeCreated,
            //    OrgPositionId = nextOrgPositionId,
            //    CreatedBy = request?.CreatedBy,
            //    CreatedAt = DateTimeOffset.Now
            //};

            //var newHistoryApplicationForm = new HistoryApplicationForm
            //{
            //    Id = Guid.NewGuid(),
            //    ApplicationFormId = newApplicationForm.Id,
            //    Action = "Created",
            //    UserCodeAction = request?.UserCodeCreated,
            //    ActionBy = request?.CreatedBy,
            //    ActionAt = DateTimeOffset.Now,
            //};

            //if (request?.CreateLeaveRequestDto.Count > 0) //dữ liệu nhập bằng tay
            //{
            //    foreach (var leave in request.CreateLeaveRequestDto)
            //    {
            //        userCodeReceiveEmail.Add(leave?.UserCode ?? "");

            //        var newApplicationFormItem = new ApplicationFormItem
            //        {
            //            Id = Guid.NewGuid(),
            //            ApplicationFormId = newApplicationForm.Id,
            //            UserCode = leave?.UserCode,
            //            UserName = leave?.UserName,
            //            Status = true,
            //            CreatedAt = DateTimeOffset.Now
            //        };

            //        applicationFormItems.Add(newApplicationFormItem);

            //        var newLeaveRequest = new Domain.Entities.LeaveRequest
            //        {
            //            Id = Guid.NewGuid(),
            //            ApplicationFormItemId = newApplicationFormItem.Id,
            //            UserCode = leave?.UserCode,
            //            UserName = leave?.UserName,
            //            DepartmentId = leave?.DepartmentId,
            //            Position = leave?.Position,
            //            FromDate = leave?.FromDate,
            //            ToDate = leave?.ToDate,
            //            TypeLeaveId = leave?.TypeLeaveId,
            //            TimeLeaveId = leave?.TimeLeaveId,
            //            Reason = leave?.Reason,
            //            CreatedAt = DateTimeOffset.Now
            //        };

            //        if (leave?.Image != null)
            //        {
            //            using (var memoryStream = new MemoryStream())
            //            {
            //                await leave.Image.CopyToAsync(memoryStream);
            //                var imageData = memoryStream.ToArray();

            //                newLeaveRequest.Image = imageData;
            //            }
            //        }

            //        leaveRequests.Add(newLeaveRequest);
            //    }
            //}
            //else //import bằng excel
            //{
            //    var orgUnitDepartments = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            //    using var workbook = new XLWorkbook(request?.File?.OpenReadStream());
            //    var worksheet = workbook.Worksheet(1);

            //    var resultApplicationFormItemAndLeaveRequets = await ValidateExcel(worksheet, newApplicationForm.Id);

            //    applicationFormItems = resultApplicationFormItemAndLeaveRequets.applicationFormItems;
            //    leaveRequests = resultApplicationFormItemAndLeaveRequets.leaveRequests;
            //}

            ////transaction
            //using var transaction = await _context.Database.BeginTransactionAsync();

            //try
            //{
            //    _context.ApplicationForms.Add(newApplicationForm);
            //    _context.HistoryApplicationForms.Add(newHistoryApplicationForm);
            //    _context.ApplicationFormItems.AddRange(applicationFormItems);
            //    _context.LeaveRequests.AddRange(leaveRequests);

            //    await _context.SaveChangesAsync();
            //    await transaction.CommitAsync();
            //}
            //catch
            //{
            //    await transaction.RollbackAsync();
            //    throw new ValidationException("Server error!");
            //}

            //#region Send Email

            //userCodeReceiveEmail.Add(request?.EmailCreated ?? "");

            ////gửi email cho người nộp đơn và người xin nghỉ
            //List<string> emailSendNoti = (List<string>)await _context.Database.GetDbConnection().QueryAsync<string>($@"
            //    SELECT COALESCE(NULLIF(U.Email, ''), NV.NVEmail, '') AS Email FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
            //    LEFT JOIN {Global.DbWeb}.dbo.user_configs AS UC ON NV.NVMaNV = UC.UserCode 
            //    LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
            //    WHERE NV.NVMaNV IN @UserCodes
            //    AND (UC.UserCode IS NULL OR (UC.UserCode IS NOT NULL AND UC.[Key] = 'RECEIVE_MAIL_LEAVE_REQUEST' AND UC.Value = 'true'))
            //    ", new { UserCodes = userCodeReceiveEmail.Distinct().ToList() });

            //string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/leave/view/{newApplicationForm.Id}";
            //string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/leave/approval/{newApplicationForm.Id}";

            ////gửi email cho người nộp đơn và người xin nghỉ
            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailRequestHasBeenSent(
            //        emailSendNoti.ToList(),
            //        null,
            //        "Leave request has been submitted.",
            //        TemplateEmail.SendContentEmail("Leave request has been submitted.", urlView, newApplicationForm.Code),
            //        null,
            //        true
            //    )
            //);

            ////gửi email cho hr
            //if (status == (int)StatusApplicationFormEnum.WAIT_HR)
            //{
            //    var permissionHrMngLeaveRequest = await _context.Permissions
            //        .Include(e => e.UserPermissions)
            //        .FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

            //    var hr = await _userService.GetMultipleUserViclockByOrgPositionId(-1, permissionHrMngLeaveRequest?.UserPermissions?.Select(e => e.UserCode ?? "")?.ToList());

            //    BackgroundJob.Enqueue<IEmailService>(job =>
            //        job.SendEmailRequestHasBeenSent(
            //            hr.Select(e => e.Email ?? "").ToList(),
            //            null,
            //            "Request for leave request approval",
            //            TemplateEmail.SendContentEmail("Request for leave request approval", urlApproval, newApplicationForm.Code),
            //            null,
            //            true
            //        )
            //    );
            //}
            //else //gửi email cho người duyệt tiếp theo
            //{
            //    var nextUserOrgPositions = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId);

            //    BackgroundJob.Enqueue<IEmailService>(job =>
            //        job.SendEmailManyPeopleLeaveRequest(
            //            nextUserOrgPositions.Select(e => e.Email ?? "").ToList(),
            //            null,
            //            "Request for leave request approval",
            //            TemplateEmail.SendContentEmail("Request for leave request approval", urlApproval, newApplicationForm.Code),
            //            null,
            //            true
            //        )
            //    );
            //}

            //#endregion

            //return true;
        }

        private async Task<(int nextOrgPositionId, int status)> GetNextOrgPositionAndStatusLeaveRequest(int orgPositionId, string type = "create", int? orgPositionIdApplicationForm = null) //type approval or create
        {
            return (1,1);
            //int nextOrgPositionId = 0;
            //int status = (int)StatusApplicationFormEnum.PENDING;

            //var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            //var approvalFlows = await _context.ApprovalFlows
            //    .Where(e => e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST && e.FromOrgPositionId == orgPositionId)
            //    .FirstOrDefaultAsync();

            //var orgPosition = await _context.OrgPositions
            //    .Include(e => e.Unit)
            //    .FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            //if (approvalFlows != null)
            //{

            //}
            //else if (approvalFlows == null)
            //{
            //    //1. là general manager, 2 là trường hợp approval của manager thì gửi đến hr
            //    if (
            //        orgPosition.UnitId == (int)UnitEnum.GM ||
            //        (type == "approved" && orgPositionIdApplicationForm != null && orgPositionIdApplicationForm == orgPositionId && orgPosition.UnitId == (int)UnitEnum.Manager)
            //    )
            //    {
            //        nextOrgPositionId = 0;
            //        status = (int)StatusApplicationFormEnum.WAIT_HR;
            //    }
            //    else
            //    {
            //        nextOrgPositionId = orgPosition?.ParentOrgPositionId ?? 0;
            //        status = (int)StatusApplicationFormEnum.PENDING;
            //    }
            //}

            //return (nextOrgPositionId, status);
        }

        //private async Task<(List<ApplicationFormItem> applicationFormItems, List<Domain.Entities.LeaveRequest> leaveRequests)> ValidateExcel(IXLWorksheet worksheet, Guid applicationFormId)
        //{
            //List<string> checkDepartments = [];
            //List<string> checkUserCodesCanLeaveRq = [];

            //List<ApplicationFormItem> applicationFormItems = [];
            //List<Domain.Entities.LeaveRequest> leaveRequests = [];

            //Helper.ValidateExcelHeader(worksheet, ["Mã nhân viên", "Họ tên", "Bộ phận", "Chức vụ", "Loại phép", "Thời gian nghỉ", "Nghỉ từ ngày", "Nghỉ đến ngày", "Lý do"]);

            //var rows = (worksheet?.RangeUsed()?.RowsUsed().Skip(2)) ?? throw new ValidationException("Không có dữ liệu nào, kiểm tra lại file excel");

            //var orgUnitDepartments = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();
            //var timeLeaves = await _context.TimeLeaves.ToListAsync();
            //var typeLeaves = await _context.TypeLeaves.ToListAsync();

            //int currentRow = 3;

            //foreach (var row in rows)
            //{
            //    string userCode = row.Cell(1).GetValue<string>();
            //    string userName = row.Cell(2).GetValue<string>();
            //    string department = row.Cell(3).GetValue<string>();
            //    string position = row.Cell(4).GetValue<string>();
            //    string typeLeave = row.Cell(5).GetValue<string>();
            //    string timeLeave = row.Cell(6).GetValue<string>();
            //    string strFromDate = row.Cell(7).GetValue<string>();
            //    string strToDate = row.Cell(7).GetValue<string>();
            //    DateTimeOffset fromDate;
            //    DateTimeOffset toDate;
            //    string reason = row.Cell(9).GetValue<string>();

            //    bool isEmptyRow = string.IsNullOrWhiteSpace(userCode)
            //        && string.IsNullOrWhiteSpace(userName)
            //        && string.IsNullOrWhiteSpace(department)
            //        && string.IsNullOrWhiteSpace(position)
            //        && string.IsNullOrWhiteSpace(typeLeave)
            //        && string.IsNullOrWhiteSpace(timeLeave)
            //        && string.IsNullOrWhiteSpace(strFromDate)
            //        && string.IsNullOrWhiteSpace(strToDate)
            //        && string.IsNullOrWhiteSpace(reason);

            //    if (isEmptyRow)
            //    {
            //        break;
            //    }

            //    var errors = new List<string>();

            //    if (string.IsNullOrWhiteSpace(userCode))
            //        errors.Add("Mã nhân viên không được để trống");

            //    if (string.IsNullOrWhiteSpace(userName))
            //        errors.Add("Họ tên không được để trống");

            //    if (string.IsNullOrWhiteSpace(department))
            //        errors.Add("Phòng ban không được để trống");

            //    if (string.IsNullOrWhiteSpace(position))
            //        errors.Add("Chức vụ không được để trống");

            //    if (string.IsNullOrWhiteSpace(typeLeave))
            //        errors.Add("Loại nghỉ phép không được để trống");

            //    if (string.IsNullOrWhiteSpace(timeLeave))
            //        errors.Add("Số giờ/ngày nghỉ không được để trống");

            //    if (!DateTimeOffset.TryParse(row.Cell(7).GetValue<string>(), out fromDate))
            //        errors.Add("Ngày bắt đầu không hợp lệ");

            //    if (!DateTimeOffset.TryParse(row.Cell(8).GetValue<string>(), out toDate))
            //        errors.Add("Ngày kết thúc không hợp lệ");

            //    if (string.IsNullOrWhiteSpace(reason))
            //        errors.Add("Lý do không được để trống");

            //    if (errors.Any())
            //    {
            //        throw new ValidationException($"Lỗi ở dòng {currentRow}: {string.Join("; ", errors)}");
            //    }

            //    checkUserCodesCanLeaveRq.Add(userCode);

            //    int departmentId = orgUnitDepartments?.FirstOrDefault(e => e.Name == department)?.Id
            //        ?? throw new ValidationException($"Lỗi ở dòng {currentRow}, phòng ban không chính xác");

            //    int timeLeaveId = timeLeaves?.FirstOrDefault(e => e.Id == (timeLeave == "CN" ? 1 : timeLeave == "S" ? 2 : 3))?.Id
            //        ?? throw new ValidationException($"Lỗi ở dòng {currentRow}, thời gian nghỉ không chính xác");

            //    int typeLeaveId = typeLeaves?.FirstOrDefault(e => e.Code?.ToLower() == typeLeave.ToLower())?.Id
            //        ?? throw new ValidationException($"Lỗi ở dòng {currentRow}, loại nghỉ phép không chính xác");

            //    var newApplicationFormItem = new ApplicationFormItem
            //    {
            //        Id = Guid.NewGuid(),
            //        ApplicationFormId = applicationFormId,
            //        UserCode = userCode,
            //        UserName = userName,
            //        Status = true,
            //        CreatedAt = DateTimeOffset.Now
            //    };

            //    applicationFormItems.Add(newApplicationFormItem);

            //    var newLeaveRequest = new Domain.Entities.LeaveRequest
            //    {
            //        Id = Guid.NewGuid(),
            //        ApplicationFormItemId = newApplicationFormItem.Id,
            //        UserCode = userCode,
            //        UserName = userName,
            //        DepartmentId = departmentId,
            //        Position = position,
            //        FromDate = fromDate,
            //        ToDate = toDate,
            //        TypeLeaveId = typeLeaveId,
            //        TimeLeaveId = timeLeaveId,
            //        Reason = reason,
            //        CreatedAt = DateTimeOffset.Now
            //    };

            //    leaveRequests.Add(newLeaveRequest);

            //    currentRow++;
            //}

            ////check xem co ton tai khong
            ////checkUserCodesCanLeaveRq
            //return (applicationFormItems, leaveRequests);
        //}

        public async Task<PagedResults<MyLeaveRequestResponse>> GetMyLeaveRequest(MyLeaveRequest request)
        {
            return null;
            //var parameters = new DynamicParameters();

            //parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            //parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            //parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            //parameters.Add("@Status", request.Status, DbType.Int32, ParameterDirection.Input);
            //parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            //var results = await _context.Database.GetDbConnection()
            //    .QueryAsync<MyLeaveRequestResponse>(
            //        "dbo.Leave_GET_GetMyLeave",
            //        parameters,
            //        commandType: CommandType.StoredProcedure
            //);

            //int totalRecords = parameters.Get<int>("@TotalRecords");
            //int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            //return new PagedResults<MyLeaveRequestResponse>
            //{
            //    Data = (List<MyLeaveRequestResponse>)results,
            //    TotalItems = totalRecords,
            //    TotalPages = totalPages
            //};
        }

        public async Task<PagedResults<MyLeaveRequestRegisteredResponse>> GetMyLeaveRequestRegistered(MyLeaveRequestRegistered request)
        {
            return null;
            //var query = _context.ApplicationForms.AsQueryable().Where(e => e.UserCodeCreatedBy == request.UserCode && e.DeletedAt == null && e.RequestTypeId == (int)RequestTypeEnum.LEAVE_REQUEST);

            //var totalItem = await query.CountAsync();

            //var results = await query
            //    .OrderByDescending(e => e.CreatedAt)
            //    .Include(e => e.RequestStatus)
            //    .Include(e => e.RequestType)
            //    .Select(x => new MyLeaveRequestRegisteredResponse
            //    {
            //        Id = x.Id,
            //        Code = x.Code,
            //        CreatedBy = x.CreatedBy,
            //        CreatedAt = x.CreatedAt,
            //        RequestStatus = x.RequestStatus,
            //        RequestType = x.RequestType
            //    })
            //    .ToListAsync();

            //int totalPages = (int)Math.Ceiling((double)totalItem / request.PageSize);

            //return new PagedResults<MyLeaveRequestRegisteredResponse>
            //{
            //    Data = results,
            //    TotalItems = totalItem,
            //    TotalPages = totalPages
            //};
        }

        public async Task<object> DeleteApplicationFormLeave(Guid applicationFormId)
        {
            return null;
            //var now = DateTimeOffset.Now;

            //await _context.LeaveRequests
            //    .Where(lr => _context.ApplicationFormItems
            //        .Where(afi => afi.ApplicationFormId == applicationFormId)
            //        .Select(afi => afi.Id)
            //        .Contains(lr.ApplicationFormItemId ?? Guid.Empty))
            //    .ExecuteUpdateAsync(s => s.SetProperty(lr => lr.DeletedAt, now));

            //await _context.ApplicationFormItems
            //    .Where(afi => afi.ApplicationFormId == applicationFormId)
            //    .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, now));

            //await _context.ApplicationForms
            //    .Where(af => af.Id == applicationFormId)
            //    .ExecuteUpdateAsync(s => s.SetProperty(af => af.DeletedAt, now));

            //return true;
        }

        public async Task<List<Domain.Entities.LeaveRequest>> GetListLeaveToUpdate(Guid Id)
        {
            return null;
            //return await _context.LeaveRequests
            //    .Include(e => e.OrgUnit)
            //    .Where(e => e.ApplicationFormItem != null && (e.Id == Id || e.ApplicationFormItem.ApplicationFormId == Id))
            //    .ToListAsync();
        }

        public async Task<object> Update(Guid id, List<CreateLeaveRequestDto> listLeaveRequests)
        {
            return null;
            //var applicationFormId = id;

            ////delete
            //var leaveDeletes = await _context.LeaveRequests
            //    .Include(e => e.ApplicationFormItem)
            //    .Where(e => e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == applicationFormId && !listLeaveRequests.Select(e => e.Id).Contains(e.Id))
            //    .ToListAsync();

            //await _context.ApplicationFormItems
            //    .Where(afi => leaveDeletes.Select(l => l.ApplicationFormItemId).Contains(afi.Id))
            //    .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, DateTimeOffset.Now));

            //await _context.LeaveRequests
            //    .Where(e => leaveDeletes.Select(l => l.Id).Contains(e.Id))
            //    .ExecuteUpdateAsync(s => s.SetProperty(afi => afi.DeletedAt, DateTimeOffset.Now));

            ////update
            //var existingLeaves = await _context.LeaveRequests.Where(l => listLeaveRequests.Select(u => u.Id).Contains(l.Id)).ToListAsync();

            //foreach (var itemUpdateLeave in listLeaveRequests)
            //{
            //    var leave = existingLeaves.FirstOrDefault(x => x.Id == itemUpdateLeave.Id);

            //    if (leave != null)
            //    {
            //        leave.Position = itemUpdateLeave.Position;
            //        leave.FromDate = itemUpdateLeave.FromDate;
            //        leave.ToDate = itemUpdateLeave.ToDate;
            //        leave.TypeLeaveId = itemUpdateLeave.TypeLeaveId;
            //        leave.TimeLeaveId = itemUpdateLeave.TimeLeaveId;
            //        leave.Reason = itemUpdateLeave.Reason;
            //        leave.UpdateAt = DateTimeOffset.Now;

            //        _context.LeaveRequests.Update(leave);
            //    }
            //    else //add new
            //    {
            //        var newApplicationFormItem = new ApplicationFormItem
            //        {
            //            Id = Guid.NewGuid(),
            //            ApplicationFormId = applicationFormId,
            //            UserCode = itemUpdateLeave?.UserCode,
            //            UserName = itemUpdateLeave?.UserName,
            //            Status = true,
            //            CreatedAt = DateTimeOffset.Now
            //        };

            //        var newLeave = new Domain.Entities.LeaveRequest
            //        {
            //            Id = Guid.NewGuid(),
            //            ApplicationFormItemId = newApplicationFormItem.Id,
            //            UserCode = itemUpdateLeave?.UserCode,
            //            UserName = itemUpdateLeave?.UserName,
            //            DepartmentId = itemUpdateLeave?.DepartmentId,
            //            Position = itemUpdateLeave?.Position,
            //            FromDate = itemUpdateLeave?.FromDate,
            //            ToDate = itemUpdateLeave?.ToDate,
            //            TypeLeaveId = itemUpdateLeave?.TypeLeaveId,
            //            TimeLeaveId = itemUpdateLeave?.TimeLeaveId,
            //            Reason = itemUpdateLeave?.Reason,
            //            CreatedAt = DateTimeOffset.Now
            //        };
            //        _context.ApplicationFormItems.Add(newApplicationFormItem);
            //        _context.LeaveRequests.Add(newLeave);
            //    }
            //}

            //await _context.SaveChangesAsync();

            //return true;
        }

        public async Task<ViewDetailLeaveRequestWithHistoryResponse?> ViewDetailLeaveRequestWithHistory(Guid Id)
        {
            return null;
            //var leaveRequest = await _context.LeaveRequests
            //    .Include(e => e.TimeLeave).Include(e => e.TypeLeave).Include(e => e.ApplicationFormItem).Include(e => e.OrgUnit)
            //    .FirstOrDefaultAsync(e => e.Id == Id) ?? throw new NotFoundException("Leave request not found");

            //var applicationFormId = leaveRequest?.ApplicationFormItem?.ApplicationFormId;

            //if (leaveRequest != null)
            //{
            //    leaveRequest.ApplicationFormItem = null;
            //}

            //var applicationForm = await _context.ApplicationForms
            //    .Select(x => new ApplicationForm
            //    {
            //        Id = x.Id,
            //        Code = x.Code,
            //        RequestTypeId = x.RequestTypeId,
            //        RequestStatusId = x.RequestStatusId,
            //        OrgPositionId = x.OrgPositionId,
            //        UserCodeCreatedBy = x.UserCodeCreatedBy,
            //        CreatedBy =  x.CreatedBy,
            //        Note = x.Note,
            //        Step = x.Step,
            //        MetaData = x.MetaData,
            //        CreatedAt = x.CreatedAt,
            //        HistoryApplicationForms = x.HistoryApplicationForms.Select(h => new HistoryApplicationForm
            //        {
            //            Id = h.Id,
            //            ApplicationFormId = h.ApplicationFormId,
            //            Note = h.Note,
            //            Action = h.Action,
            //            ActionBy = h.ActionBy,
            //            ActionAt = h.ActionAt
            //        })
            //        .ToList()
            //    })
            //    .FirstOrDefaultAsync(e => e.Id == applicationFormId);

            //var result = new ViewDetailLeaveRequestWithHistoryResponse
            //{
            //    Id = leaveRequest?.Id,
            //    ApplicationFormItemId = leaveRequest?.ApplicationFormItemId,
            //    UserCode = leaveRequest?.UserCode,
            //    UserName = leaveRequest?.UserName,
            //    DepartmentId = leaveRequest?.DepartmentId,
            //    Position = leaveRequest?.Position,
            //    FromDate = leaveRequest?.FromDate,
            //    ToDate = leaveRequest?.ToDate,
            //    TypeLeaveId = leaveRequest?.TypeLeaveId,
            //    TimeLeaveId = leaveRequest?.TimeLeaveId,
            //    Reason = leaveRequest?.Reason,
            //    NoteOfHR = leaveRequest?.NoteOfHR,
            //    CreatedAt = leaveRequest?.CreatedAt,
            //    OrgUnit = leaveRequest?.OrgUnit,
            //    TimeLeave = leaveRequest?.TimeLeave,
            //    TypeLeave = leaveRequest?.TypeLeave,
            //    ApplicationForm = applicationForm
            //};

            //return result;
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
        /// Lấy danh sách những hr có quyền quản lý nghỉ phép, có thể bỏ
        /// </summary>
        //public async Task<List<HrMngLeaveRequestResponse>> GetHrWithManagementLeavePermission()
        //{
        //    var permissionHrMngLeaveRequest = await _context.Permissions.FirstOrDefaultAsync(e => e.Name == "leave_request.hr_management_leave_request");

        //    if (permissionHrMngLeaveRequest == null)
        //    {
        //        throw new NotFoundException("Permission hr manage leave request not found");
        //    }

        //    var connection = (SqlConnection)_context.CreateConnection();

        //    if (connection.State != ConnectionState.Open)
        //    {
        //        await connection.OpenAsync();
        //    }

        //    var userCodePerission = await _context.UserPermissions.Where(e => e.PermissionId == permissionHrMngLeaveRequest.Id).Select(e => e.UserCode).ToListAsync();

        //    var sql = $@"
        //        SELECT
        //             NV.NVMaNV,
        //             {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) as NVHoTen,
        //             BP.BPMa,
        //             {Global.DbViClock}.dbo.funTCVN2Unicode(BP.BPTen) as BPTen,
        //             COALESCE(NULLIF(Email, ''), NVEmail, '') AS Email
        //        FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
        //        LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan as BP ON NV.NVMaBP = BP.BPMa
        //        LEFT JOIN {Global.DbWeb}.dbo.users AS U ON NV.NVMaNV = U.UserCode
        //        WHERE NV.NVMaNV IN @userCodePerission
        //    ";

        //    var param = new
        //    {
        //        userCodePerission = userCodePerission
        //    };

        //    var result = await connection.QueryAsync<HrMngLeaveRequestResponse>(sql, param);

        //    return (List<HrMngLeaveRequestResponse>)result;
        //}

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
