using System.Data;
using System.Text.Json;
using Dapper;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Dtos.ITForm.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;

namespace ServicePortals.Application.Services.ITForm
{
    public class ITFormServiceImpl : Interfaces.ITForm.ITFormService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IOrgPositionService _orgPositionService;
        private readonly IConfiguration _configuration;
        public ITFormServiceImpl(ApplicationDbContext context, IUserService userService, IOrgPositionService orgPositionService, IConfiguration configuration)
        {
            _context = context;
            _userService = userService;
            _orgPositionService = orgPositionService;
            _configuration = configuration;
        }

        public async Task<PagedResults<Domain.Entities.ITForm>> GetAll(GetAllITFormRequest request)
        {
            //string? userCode = request.UserCode;
            //int page = request.Page;
            //int pageSize = request.PageSize;
            //int? departmentId = request.DepartmentId;
            //int? statusId = request.RequestStatusId;
            //int? year = request.Year;

            //var query = _context.ITForms.AsQueryable();

            //if (!string.IsNullOrWhiteSpace(userCode))
            //{
            //    query = query.Where(e => e.ApplicationForm != null && (e.ApplicationForm.UserCodeRequestor == userCode || e.ApplicationForm.UserCodeCreated == userCode) && e.DeletedAt == null);
            //}

            //if (departmentId != null)
            //{
            //    query = query.Where(e => e.DepartmentId == departmentId);
            //}

            //if (year != null)
            //{
            //    query = query.Where(e => e.CreatedAt != null && e.CreatedAt.Value.Year == year);
            //}

            //if (statusId != null)
            //{
            //    if (statusId == (int)StatusApplicationFormEnum.PENDING || statusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL)
            //    {
            //        query = query.Where(e => e.ApplicationForm != null &&
            //            (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.PENDING ||
            //             e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL));
            //    }
            //    else if (statusId == (int)StatusApplicationFormEnum.IN_PROCESS || statusId == (int)StatusApplicationFormEnum.ASSIGNED)
            //    {
            //        query = query.Where(e => e.ApplicationForm != null &&
            //            (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS ||
            //             e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.ASSIGNED));
            //    }
            //    else
            //    {
            //        query = query.Where(e => e.ApplicationForm != null &&
            //            e.ApplicationForm.RequestStatusId == statusId);
            //    }
            //}

            //var totalItems = await query.CountAsync();

            //var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            //var results = await SelectITForm(query)
            //    .OrderByDescending(e => e.CreatedAt)
            //    .Skip(((page - 1) * pageSize)).Take(pageSize)
            //    .ToListAsync();

            //return new PagedResults<Domain.Entities.ITForm>
            //{
            //    Data = results,
            //    TotalItems = totalItems,
            //    TotalPages = totalPages
            //};
            return null;
        }

        public async Task<Domain.Entities.ITForm?> GetById(Guid Id)
        {
            var query = _context.ITForms.AsQueryable();

            var result = await SelectITForm(query).FirstOrDefaultAsync(e => e.Id == Id);

            return result;
        }

        public async Task<object> Create(CreateITFormRequest request)
        {
            //if (request.DepartmentId == null || request.OrgPositionId <= 0)
            //{
            //    throw new ValidationException(Global.UserNotSetInformation);
            //}

            //int? nextOrgPositionId = -1;

            //int status = (int)StatusApplicationFormEnum.PENDING;

            //var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            //var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.FromOrgPositionId == orgPosition.Id);

            ////nếu có custom approval flow
            //if (approvalFlowCurrentPositionId != null)
            //{
            //    nextOrgPositionId = approvalFlowCurrentPositionId.ToOrgPositionId;

            //    if (approvalFlowCurrentPositionId.IsFinal == true)
            //    {
            //        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //    }
            //}
            //else
            //{
            //    //manager các bộ phận
            //    if (orgPosition?.ParentOrgPositionId == null)
            //    {
            //        var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.PositonContext == "MANAGER").ToListAsync();

            //        //nếu như đơn có các trạng thái như Server,... thì cần qua sếp trên, nếu k thì qua luôn manager IT
            //        var finalStep = approvalFlows
            //            .FirstOrDefault(step => !string.IsNullOrEmpty(step.Condition) &&
            //                JsonDocument.Parse(step.Condition)
            //                    .RootElement.GetProperty("it_category")
            //                    .EnumerateArray()
            //                    .Select(e => e.GetProperty("id").GetInt32())
            //                    .Any(id => request.ITCategories.Contains(id)))
            //            ?? approvalFlows.FirstOrDefault(step => step.Condition == null);

            //        nextOrgPositionId = finalStep?.ToOrgPositionId;

            //        if (finalStep?.IsFinal == true)
            //        {
            //            status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //        }
            //    }
            //    else
            //    {
            //        var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(request.OrgPositionId);
            //        nextOrgPositionId = getManagerOrgPostionId?.Id;
            //    }
            //}

            //var applicationForm = new ApplicationForm
            //{
            //    Id = Guid.NewGuid(),
            //    Code = Helper.GenerateFormCode("FIT"),
            //    UserCodeCreatedBy = request.UserCodeRequestor,
            //    CreatedBy = request.UserNameRequestor,
            //    RequestTypeId = (int)RequestTypeEnum.FORM_IT,
            //    OrgPositionId = nextOrgPositionId,
            //    RequestStatusId = status,
            //    CreatedAt = DateTimeOffset.Now,
            //};

            //var historyApplicationForm = new HistoryApplicationForm
            //{
            //    Id = Guid.NewGuid(),
            //    ApplicationFormId = applicationForm.Id,
            //    Action = "Created",
            //    ActionBy = request?.UserNameRequestor,
            //    UserCodeAction = request.UserCodeCreated,
            //    ActionAt = DateTimeOffset.Now,
            //};

            //var applicationFormItem = new ApplicationFormItem
            //{
            //    Id = Guid.NewGuid(),
            //    ApplicationFormId = applicationForm.Id,
            //    UserCode = request?.UserCodeCreated,
            //    UserName = request?.CreatedBy,
            //    Status = true,
            //    CreatedAt = DateTimeOffset.Now
            //};

            //var formIT = new Domain.Entities.ITForm
            //{
            //    Id = Guid.NewGuid(),
            //    ApplicationFormItemId = applicationFormItem.Id,
            //    DepartmentId = request.DepartmentId,
            //    Email = request.Email,
            //    Position = request.Position,
            //    PriorityId = request.PriorityId,
            //    Reason = request.Reason,
            //    RequestDate = request.RequestDate,
            //    RequiredCompletionDate = request.RequiredCompletionDate,
            //    CreatedAt = DateTimeOffset.Now
            //};

            //List<ITFormCategory> itFormCategories = [];

            //foreach (var item in request.ITCategories)
            //{
            //    itFormCategories.Add(new ITFormCategory
            //    {
            //        ITFormId = formIT.Id,
            //        ITCategoryId = item
            //    });
            //}

            //_context.ApplicationForms.Add(applicationForm);
            //_context.ITForms.Add(formIT);
            //_context.ITFormCategories.AddRange(itFormCategories);

            //await _context.SaveChangesAsync();

            //var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            //var itemFormIT = await _context.ITForms
            //    .Include(e => e.ApplicationForm)
            //    .Include(e => e.Priority)
            //    .Include(e => e.ItFormCategories)
            //        .ThenInclude(e => e.ITCategory)
            //    .FirstOrDefaultAsync(e => e.Id == formIT.Id) ?? throw new NotFoundException("Not found data, please check again");

            //string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-form-it/{itemFormIT.Id}";

            //string bodyMail = $@"
            //    <h4>
            //        <span>Detail: </span>
            //        <a href={urlApproval}>{itemFormIT?.ApplicationForm?.Code}</a>
            //    </h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailFormIT(
            //        nextUserInfo.Select(e => e.Email ?? "").ToList(),
            //        null,
            //        "Request for IT Form approval",
            //        bodyMail,
            //        null,
            //        true
            //    )
            //);

            return true;
        }

        public async Task<object> Delete(Guid Id)
        {
            //var item = await _context.ITForms.FirstOrDefaultAsync(e => (e.Id == Id) && e.DeletedAt == null) ?? throw new NotFoundException("Form IT not found");

            //await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == item.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            //await _context.ApplicationForms.Where(e => e.Id == item.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            //await _context.ITForms.Where(e => e.Id == item.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            return true;
        }

        public async Task<object> Update(Guid Id, UpdateITFormRequest request)
        {
            var item = await _context.ITForms.FirstOrDefaultAsync(e => (e.Id == Id) && e.DeletedAt == null) ?? throw new NotFoundException("Form IT not found");

            item.Position = request.Position;
            item.Email = request.Email;
            item.RequestDate = request.RequestDate;
            item.RequiredCompletionDate = request.RequiredCompletionDate;
            item.Reason = request.Reason;
            item.PriorityId = request.PriorityId;
            item.UpdatedAt = DateTimeOffset.Now;

            _context.ITForms.Update(item);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> Approval(ApprovalRequest request)
        {
            //var itemFormIT = await _context.ITForms
            //    .Include(e => e.ApplicationForm)
            //    .Include(e => e.Priority)
            //    .Include(e => e.ItFormCategories)
            //        .ThenInclude(e => e.ITCategory)
            //    .FirstOrDefaultAsync(e => e.Id == request.ITFormId || e.ApplicationFormId == request.ITFormId) ?? throw new NotFoundException("Not found data, please check again");

            //var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == itemFormIT.ApplicationFormId) ?? throw new NotFoundException("Item application form not found");

            //if (request?.OrgPositionId != applicationForm.OrgPositionId)
            //{
            //    throw new ForbiddenException(Global.NotPermissionApproval);
            //}

            //var historyApplicationForm = new HistoryApplicationForm
            //{
            //    UserCodeApproval = request?.UserCodeApproval,
            //    UserNameApproval = request?.UserNameApproval,
            //    ApplicationFormId = applicationForm?.Id,
            //    Note = request?.Note,
            //    CreatedAt = DateTimeOffset.Now
            //};

            //List<int?>? ITFormCategoryIds = itemFormIT.ItFormCategories.Select(e => e.ITCategoryId)?.ToList();

            //if (request?.Status == false)
            //{
            //    applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
            //    applicationForm.UpdatedAt = DateTimeOffset.Now;
            //    historyApplicationForm.Action = "REJECT";

            //    _context.ApplicationForms.Update(applicationForm);
            //    _context.HistoryApplicationForms.Add(historyApplicationForm);

            //    await _context.SaveChangesAsync();

            //    string? reasonReject = request?.Note == null || request.Note == "" ? "--" : request?.Note;

            //    string bodyMailReject = $@"<h4><span style=""color:red"">Reason: {reasonReject}</span></h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            //    BackgroundJob.Enqueue<IEmailService>(job =>
            //        job.SendEmailFormIT(
            //            new List<string> { itemFormIT.Email ?? "" },
            //            null,
            //            "Your request IT form has been rejected",
            //            bodyMailReject,
            //            null,
            //            true
            //        )
            //    );

            //    return true;
            //}

            //int? nextOrgPositionId = 0;

            //int status = (int)StatusApplicationFormEnum.IN_PROCESS;

            //var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException("Bạn chưa được cập nhật vị trí, liên hệ với HR");

            //var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.FromOrgPositionId == orgPosition.Id);

            ////nếu có custom approval flow
            //if (approvalFlowCurrentPositionId != null)
            //{
            //    nextOrgPositionId = approvalFlowCurrentPositionId.ToOrgPositionId;

            //    if (approvalFlowCurrentPositionId.IsFinal == true)
            //    {
            //        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //    }
            //}
            //else
            //{
            //    //manager các bộ phận
            //    if (orgPosition?.ParentOrgPositionId == null)
            //    {
            //        var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.PositonContext == "MANAGER").ToListAsync();

            //        //nếu như đơn có các trạng thái như Server,... thì cần qua sếp trên, nếu k thì qua luôn manager IT
            //        var finalStep = approvalFlows
            //            .FirstOrDefault(step => !string.IsNullOrEmpty(step.Condition) &&
            //                JsonDocument.Parse(step.Condition)
            //                    .RootElement.GetProperty("it_category")
            //                    .EnumerateArray()
            //                    .Select(e => e.GetProperty("id").GetInt32())
            //                    .Any(id => ITFormCategoryIds != null && ITFormCategoryIds.Contains(id)))
            //            ?? approvalFlows.FirstOrDefault(step => step.Condition == null);

            //        nextOrgPositionId = finalStep?.ToOrgPositionId;

            //        if (finalStep?.IsFinal == true)
            //        {
            //            status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //        }
            //    }
            //    else
            //    {
            //        var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(request?.OrgPositionId ?? -1);
            //        nextOrgPositionId = getManagerOrgPostionId?.Id;
            //    }
            //}

            //applicationForm.RequestStatusId = status;
            //applicationForm.OrgPositionId = nextOrgPositionId;
            //applicationForm.UpdatedAt = DateTimeOffset.Now;
            //historyApplicationForm.Action = "APPROVAL";

            //_context.ApplicationForms.Update(applicationForm);
            //_context.HistoryApplicationForms.Add(historyApplicationForm);

            //await _context.SaveChangesAsync();

            //var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            //string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-form-it/{itemFormIT.Id}";

            //string bodyMail = $@"
            //    <h4>
            //        <span>Detail: </span>
            //        <a href={urlApproval}>{itemFormIT?.ApplicationForm?.Code}</a>
            //    </h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailFormIT(
            //        nextUserInfo.Select(e => e.Email ?? "").ToList(),
            //        null,
            //        "Request for IT Form approval",
            //        bodyMail,
            //        null,
            //        true
            //    )
            //);

            return true;
        }

        public async Task<object> AssignedTask(AssignedTaskRequest request)
        {
            //var itemITForm = await _context.ITForms
            //    .Include(e => e.Priority)
            //    .Include(e => e.ItFormCategories)
            //        .ThenInclude(e => e.ITCategory)
            //    .FirstOrDefaultAsync(e => e.Id == request.ITFormId || e.ApplicationFormId == request.ITFormId) ?? throw new NotFoundException("IT Form not found");

            //var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == itemITForm.ApplicationFormId) ?? throw new NotFoundException("Application form not found");

            //if (request?.OrgPositionId != applicationForm.OrgPositionId)
            //{
            //    throw new ForbiddenException(Global.NotPermissionApproval);
            //}

            //applicationForm.UpdatedAt = DateTimeOffset.Now;
            //applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.ASSIGNED; //set sang trạng thái là đẫ được gắn task
            //applicationForm.OrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_AFTER_ASSIGNED_TASK; //SET -1 để không còn ai, dựa vào usercode để xử lý tiếp theo
            //_context.ApplicationForms.Update(applicationForm);

            //itemITForm.NoteManagerIT = request.NoteManager;
            //_context.ITForms.Update(itemITForm);

            //List<AssignedTask> assignedTasks = [];
            //foreach (var itemUserAssigned in request.UserAssignedTasks)
            //{
            //    assignedTasks.Add(new AssignedTask
            //    {
            //        ApplicationFormId = applicationForm.Id,
            //        UserCode = itemUserAssigned.UserCode
            //    });
            //}
            //_context.AssignTasks.AddRange(assignedTasks);

            //var historyApplicationForm = new HistoryApplicationForm
            //{
            //    UserCodeApproval = request.UserCodeApproval,
            //    UserNameApproval = request.UserNameApproval,
            //    ApplicationFormId = applicationForm.Id,
            //    Action = "APPROVAL",
            //    Note = request.NoteManager,
            //    CreatedAt = DateTimeOffset.Now
            //};

            //_context.HistoryApplicationForms.Add(historyApplicationForm);

            //await _context.SaveChangesAsync();

            //string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/assigned-form-it/{itemITForm.Id}";

            //string bodyMail = $@"
            //    <h4>
            //        <span>Detail: </span>
            //        <a href={urlApproval}>{itemITForm?.ApplicationForm?.Code}</a>
            //    </h4>" + TemplateEmail.EmailFormIT(itemITForm);

            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailFormIT(
            //        request.UserAssignedTasks.Select(e => e.Email ?? "").ToList(),
            //        null,
            //        "New Task Assigned",
            //        bodyMail,
            //        null,
            //        true
            //    )
            //);

            return true;
        }

        public async Task<object> ResolvedTask(ResolvedTaskRequest request)
        {
            //var itemFormIT = await _context.ITForms
            //    .Include(e => e.Priority)
            //    .Include(e => e.ItFormCategories)
            //        .ThenInclude(e => e.ITCategory)
            //    .FirstOrDefaultAsync(e => e.Id == request.ITFormId || e.ApplicationFormId == request.ITFormId) ?? throw new NotFoundException("Not found data, please check again");

            //var applicationForm = await _context.ApplicationForms
            //    .Include(e => e.HistoryApplicationForms)
            //    .Include(e => e.AssignedTasks)
            //    .FirstOrDefaultAsync(e => e.Id == itemFormIT.ApplicationFormId)

            //    ?? throw new NotFoundException("Not found data, please check again");

            //bool exists = applicationForm.AssignedTasks.Any(e => e.UserCode == request.UserCodeApproval);

            //if (!exists)
            //{
            //    throw new ForbiddenException(Global.NotPermissionApproval);
            //}

            //itemFormIT.TargetCompletionDate = request.TargetCompletionDate;
            //itemFormIT.ActualCompletionDate = request.ActualCompletionDate;

            //_context.ITForms.Update(itemFormIT);

            //applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;

            //applicationForm.UpdatedAt = DateTimeOffset.Now;

            //_context.ApplicationForms.Update(applicationForm);

            //var historyApplicationForm = new HistoryApplicationForm
            //{
            //    UserCodeApproval = request.UserCodeApproval,
            //    UserNameApproval = request.UserNameApproval,
            //    ApplicationFormId = applicationForm.Id,
            //    Action = "RESOLVED",
            //    CreatedAt = DateTimeOffset.Now
            //};

            //_context.HistoryApplicationForms.Add(historyApplicationForm);

            //await _context.SaveChangesAsync();

            //List<string> ccUserCode = [];
            //ccUserCode.Add(applicationForm?.HistoryApplicationForms?.First()?.UserCodeApproval ?? ""); //latest manager assigned, get usercode

            ////get usercode everybody assigned task
            //foreach (var itemAss in applicationForm!.AssignedTasks)
            //{
            //    ccUserCode.Add(itemAss.UserCode ?? "");
            //}

            ////get email to cc, manager, user assigned task
            //List<GetMultiUserViClockByOrgPositionIdResponse> multipleByUserCodes = await _userService.GetMultipleUserViclockByOrgPositionId(-1, ccUserCode);

            //string urlDetail = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/view-form-it/{itemFormIT.Id}";

            //string bodyMail = $@"
            //    <h4>
            //        <span>Detail: </span>
            //        <a href={urlDetail}>{itemFormIT?.ApplicationForm?.Code}</a>
            //    </h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            ////send email
            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.SendEmailFormIT(
            //        new List<string> { itemFormIT.Email ?? "" },
            //        multipleByUserCodes.Select(e => e.Email ?? "").ToList(),
            //        "Your IT form request has been approved",
            //        bodyMail,
            //        null,
            //        true
            //    )
            //);

            return true;
        }

        public async Task<StatisticalFormITResponse> StatisticalFormIT(int year)
        {
            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var multi = await connection.QueryMultipleAsync("GetITFormStatisticalData", new { Year = year }, commandType: CommandType.StoredProcedure);

            var result = new StatisticalFormITResponse
            {
                GroupByTotal = await multi.ReadFirstAsync<GroupByTotal>(),
                GroupRecentList = (await multi.ReadAsync<GroupRecentList>()).ToList(),
                GroupByDepartment = (await multi.ReadAsync<GroupByDepartment>()).ToList(),
                GroupByMonth = (await multi.ReadAsync<GroupByMonth>()).ToList(),
                GroupByCategory = (await multi.ReadAsync<GroupByCategory>()).ToList()
            };

            return result;
        }

        public async Task<List<InfoUserAssigned>> GetMemberITAssigned()
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sql = $@"
                SELECT
                    NVMa,
                    NVMaNV,
                    {Global.DbViClock}.dbo.funTCVN2Unicode(NVHoTen) AS NVHoTen,
                    ViTriToChucId AS OrgPositionId,
	                COALESCE(NULLIF(Email, ''), NVEmail, '') AS Email
                FROM {Global.DbViClock}.[dbo].[tblNhanVien] AS NV
                LEFT JOIN {Global.DbWeb}.dbo.users as U
	                ON NV.NVMaNV = U.UserCode
                WHERE
                    NV.NVNgayRa > GETDATE() AND NV.ViTriToChucId = 8
            ";

            var result = await connection.QueryAsync<InfoUserAssigned>(sql);

            return (List<InfoUserAssigned>)result;
        }
        private static IQueryable<Domain.Entities.ITForm> SelectITForm(IQueryable<Domain.Entities.ITForm> query)
        {
            return null;
            //return query.Select(x => new Domain.Entities.ITForm
            //{
            //    Id = x.Id,
            //    ApplicationFormId = x.ApplicationFormId,
            //    DepartmentId = x.DepartmentId,
            //    Email = x.Email,
            //    Position = x.Position,
            //    Reason = x.Reason,
            //    PriorityId = x.PriorityId,
            //    NoteManagerIT = x.NoteManagerIT,
            //    RequestDate = x.RequestDate,
            //    RequiredCompletionDate = x.RequiredCompletionDate,
            //    TargetCompletionDate = x.TargetCompletionDate,
            //    ActualCompletionDate = x.ActualCompletionDate,
            //    CreatedAt = x.CreatedAt,
            //    UpdatedAt = x.UpdatedAt,
            //    DeletedAt = x.DeletedAt,
            //    Priority = x.Priority == null ? null : new Domain.Entities.Priority
            //    {
            //        Id = x.Priority.Id,
            //        Name = x.Priority.Name,
            //        NameE = x.Priority.NameE
            //    },
            //    OrgUnit = x.OrgUnit == null ? null : new Domain.Entities.OrgUnit
            //    {
            //        Id = x.OrgUnit.Id,
            //        Name = x.OrgUnit.Name,
            //        ParentOrgUnitId = x.OrgUnit.ParentOrgUnitId
            //    },
            //    ApplicationForm = x.ApplicationForm == null ? null : new ApplicationForm
            //    {
            //        Id = x.ApplicationForm.Id,
            //        Code = x.ApplicationForm.Code,
            //        UserCodeRequestor = x.ApplicationForm.UserCodeRequestor,
            //        UserNameRequestor = x.ApplicationForm.UserNameRequestor,
            //        UserCodeCreated = x.ApplicationForm.UserCodeCreated,
            //        UserNameCreated = x.ApplicationForm.UserNameCreated,
            //        RequestStatusId = x.ApplicationForm.RequestStatusId,
            //        RequestTypeId = x.ApplicationForm.RequestTypeId,
            //        OrgPositionId = x.ApplicationForm.OrgPositionId,
            //        CreatedAt = x.ApplicationForm.CreatedAt,
            //        RequestStatus = x.ApplicationForm.RequestStatus == null ? null : new RequestStatus
            //        {
            //            Id = x.ApplicationForm.RequestStatus.Id,
            //            Name = x.ApplicationForm.RequestStatus.Name,
            //            NameE = x.ApplicationForm.RequestStatus.NameE,
            //        },
            //        RequestType = x.ApplicationForm.RequestType == null ? null : new Domain.Entities.RequestType
            //        {
            //            Id = x.ApplicationForm.RequestType.Id,
            //            Name = x.ApplicationForm.RequestType.Name,
            //            NameE = x.ApplicationForm.RequestType.NameE,
            //        },
            //        HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(e => e.CreatedAt).Select(itemHistory => new HistoryApplicationForm
            //        {
            //            Id = itemHistory.Id,
            //            UserNameApproval = itemHistory.UserNameApproval,
            //            UserCodeApproval = itemHistory.UserCodeApproval,
            //            Action = itemHistory.Action,
            //            Note = itemHistory.Note,
            //            CreatedAt = itemHistory.CreatedAt
            //        }).ToList(),
            //        AssignedTasks = x.ApplicationForm.AssignedTasks.ToList(),
            //    },
            //    ItFormCategories = x.ItFormCategories.Select(ift => new ITFormCategory
            //    {
            //        Id = ift.Id,
            //        ITCategoryId = ift.ITCategoryId,
            //        ITFormId = ift.ITFormId,
            //        ITCategory = ift.ITCategory == null ? null : new ITCategory
            //        {
            //            Id = ift.ITCategory.Id,
            //            Name = ift.ITCategory.Name,
            //            Code = ift.ITCategory.Code
            //        }
            //    }).ToList(),
            //});
        }
    }
}
