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
using GroupByTotal = ServicePortals.Application.Dtos.ITForm.Responses.GroupByTotal;
using GroupRecentList = ServicePortals.Application.Dtos.ITForm.Responses.GroupRecentList;
using GroupByDepartment = ServicePortals.Application.Dtos.ITForm.Responses.GroupByDepartment;
using GroupByMonth = ServicePortals.Application.Dtos.ITForm.Responses.GroupByMonth;

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

        public async Task<PagedResults<GetListITFormResponse>> GetAll(GetAllITFormRequest request)
        {
            string? userCode = request.UserCode;
            int page = request.Page;
            int pageSize = request.PageSize;
            int? departmentId = request.DepartmentId;
            int? statusId = request.RequestStatusId;
            int? year = request.Year;

            var parameters = new DynamicParameters();

            parameters.Add("@Page", page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", pageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", userCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@DepartmentId", departmentId, DbType.String, ParameterDirection.Input);
            parameters.Add("@StatusId", statusId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Year", year, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<GetListITFormResponse>(
                    "dbo.ITForm_GET_GetListITForm",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<GetListITFormResponse>
            {
                Data = (List<GetListITFormResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<Domain.Entities.ITForm?> GetById(Guid Id)
        {
            var query = _context.ITForms.AsQueryable();

            var applicationFormItem = await _context.ApplicationFormItems.FirstOrDefaultAsync(e => e.ApplicationFormId == Id);

            var result = await SelectITForm(query).FirstOrDefaultAsync(e => e.Id == Id || (applicationFormItem != null && e.ApplicationFormItemId == applicationFormItem.Id));

            return result;
        }

        public async Task<object> Create(CreateITFormRequest request)
        {
            if (request.DepartmentId == null || request.OrgPositionId <= 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            int? nextOrgPositionId = -1;

            int status = (int)StatusApplicationFormEnum.PENDING;

            var orgPosition = await _context.OrgPositions.Include(e => e.Unit).FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.FromOrgPositionId == orgPosition.Id);

            //nếu có custom approval flow
            if (approvalFlowCurrentPositionId != null)
            {
                nextOrgPositionId = approvalFlowCurrentPositionId.ToOrgPositionId;

                if (approvalFlowCurrentPositionId.IsFinal == true)
                {
                    status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                }
            }
            else
            {
                //manager các bộ phận
                if (orgPosition?.UnitId == (int)UnitEnum.Manager)
                {
                    var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.PositonContext == "MANAGER").ToListAsync();

                    //nếu như đơn có các trạng thái như Server,... thì cần qua sếp trên, nếu k thì qua luôn manager IT
                    var finalStep = approvalFlows
                        .FirstOrDefault(step => !string.IsNullOrEmpty(step.Condition) &&
                            JsonDocument.Parse(step.Condition)
                                .RootElement.GetProperty("it_category")
                                .EnumerateArray()
                                .Select(e => e.GetProperty("id").GetInt32())
                                .Any(id => request.ITCategories.Contains(id)))
                        ?? approvalFlows.FirstOrDefault(step => step.Condition == null);

                    nextOrgPositionId = finalStep?.ToOrgPositionId;

                    if (finalStep?.IsFinal == true)
                    {
                        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    }
                }
                else
                {
                    var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(request.OrgPositionId);
                    nextOrgPositionId = getManagerOrgPostionId?.Id;
                }
            }

            var applicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                Code = Helper.GenerateFormCode("FIT"),
                UserCodeCreatedBy = request.UserCode,
                CreatedBy = request.UserName,
                RequestTypeId = (int)RequestTypeEnum.FORM_IT,
                OrgPositionId = nextOrgPositionId,
                RequestStatusId = status,
                DepartmentId = request.DepartmentId,
                CreatedAt = DateTimeOffset.Now,
            };

            var historyApplicationForm = new HistoryApplicationForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                Action = "Created",
                ActionBy = request?.UserName,
                UserCodeAction = request?.UserCode,
                ActionAt = DateTimeOffset.Now,
            };

            var applicationFormItem = new ApplicationFormItem
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                UserCode = request?.UserCode,
                UserName = request?.UserName,
                Status = true,
                CreatedAt = DateTimeOffset.Now
            };

            var formIT = new Domain.Entities.ITForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormItemId = applicationFormItem.Id,
                DepartmentId = request?.DepartmentId,
                Email = request?.Email,
                Position = request?.Position,
                PriorityId = request?.PriorityId,
                Reason = request?.Reason,
                RequestDate = request?.RequestDate,
                RequiredCompletionDate = request?.RequiredCompletionDate,
                CreatedAt = DateTimeOffset.Now
            };

            List<ITFormCategory> itFormCategories = [];

            foreach (var item in request.ITCategories)
            {
                itFormCategories.Add(new ITFormCategory
                {
                    ITFormId = formIT.Id,
                    ITCategoryId = item
                });
            }

            _context.ApplicationForms.Add(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);
            _context.ApplicationFormItems.Add(applicationFormItem);
            _context.ITForms.Add(formIT);
            _context.ITFormCategories.AddRange(itFormCategories);

            await _context.SaveChangesAsync();

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            var itemFormIT = await _context.ITForms
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                .Include(e => e.Priority)
                .Include(e => e.ItFormCategories)
                    .ThenInclude(e => e.ITCategory)
                .FirstOrDefaultAsync(e => e.Id == formIT.Id) ?? throw new NotFoundException("Not found data, please check again");

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-form-it/{itemFormIT.Id}";

            string bodyMail = $@"
                <h4>
                    <span>Detail: </span>
                    <a href={urlApproval}>{itemFormIT?.ApplicationFormItem?.ApplicationForm?.Code}</a>
                </h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailFormIT(
                    nextUserInfo.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for IT Form approval",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> Delete(Guid Id)
        {
            var formIT = await _context.ITForms
                .Include(e => e.ApplicationFormItem)
                .FirstOrDefaultAsync(e => e.Id == Id && e.DeletedAt == null) ?? throw new NotFoundException("Form IT not found");

            await _context.HistoryApplicationForms
                .Where(e => formIT.ApplicationFormItem != null && e.ApplicationFormId == formIT.ApplicationFormItem.ApplicationFormId)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationForms
                .Where(e => formIT.ApplicationFormItem != null && e.Id == formIT.ApplicationFormItem.ApplicationFormId)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationFormItems.Where(e => e.Id == formIT.ApplicationFormItemId)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            formIT.DeletedAt = DateTimeOffset.Now;

            _context.ITForms.Update(formIT);

            await _context.SaveChangesAsync();

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
            int? orgPositionId = request?.OrgPositionId;

            if (orgPositionId == null)
            {
                throw new NotFoundException(Global.UserNotSetInformation);
            }

            var itemFormIT = await _context.ITForms
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                .Include(e => e.Priority)
                .Include(e => e.ItFormCategories)
                    .ThenInclude(e => e.ITCategory)
                .FirstOrDefaultAsync(e => e.Id == request.ITFormId || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.ITFormId)) 
                ?? throw new NotFoundException("IT Form not found, please check again");

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => itemFormIT.ApplicationFormItem != null && e.Id == itemFormIT.ApplicationFormItem.ApplicationFormId);

            if (applicationForm == null)
            {
                throw new NotFoundException("Application form, please check again");
            }

            if (request?.OrgPositionId != applicationForm.OrgPositionId)
            {
                throw new ForbiddenException(Global.NotPermissionApproval);
            }

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeAction= request?.UserCodeApproval,
                ActionBy = request?.UserNameApproval,
                ApplicationFormId = applicationForm?.Id,
                Note = request?.Note,
                ActionAt = DateTimeOffset.Now
            };

            List<int?>? ITFormCategoryIds = itemFormIT?.ItFormCategories.Select(e => e.ITCategoryId)?.ToList();

            if (request?.Status == false)
            {
                applicationForm!.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
                applicationForm!.UpdatedAt = DateTimeOffset.Now;
                historyApplicationForm.Action = "Reject";

                _context.ApplicationForms.Update(applicationForm);
                _context.HistoryApplicationForms.Add(historyApplicationForm);

                await _context.SaveChangesAsync();

                string? reasonReject = request?.Note == null || request.Note == "" ? "--" : request?.Note;

                string bodyMailReject = $@"<h4><span style=""color:red"">Reason: {reasonReject}</span></h4>" + TemplateEmail.EmailFormIT(itemFormIT);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailFormIT(
                        new List<string> { itemFormIT != null ? itemFormIT.Email ?? "" : "" },
                        null,
                        "Your request IT form has been rejected",
                        bodyMailReject,
                        null,
                        true
                    )
                );

                return true;
            }

            int? nextOrgPositionId = 0;

            int status = (int)StatusApplicationFormEnum.IN_PROCESS;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.FromOrgPositionId == orgPosition.Id);

            //nếu có custom approval flow
            if (approvalFlowCurrentPositionId != null)
            {
                nextOrgPositionId = approvalFlowCurrentPositionId.ToOrgPositionId;

                if (approvalFlowCurrentPositionId.IsFinal == true)
                {
                    status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                }
            }
            else
            {
                //manager các bộ phận
                if (orgPosition.UnitId == (int)UnitEnum.Manager)
                {
                    var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.PositonContext == "MANAGER").ToListAsync();
                    var finalStep = approvalFlows
                        .FirstOrDefault(step => !string.IsNullOrEmpty(step.Condition) &&
                            JsonDocument.Parse(step.Condition)
                                .RootElement.EnumerateArray()
                                .Select(e => e.GetProperty("id").GetInt32())
                                .Any(id => ITFormCategoryIds != null && ITFormCategoryIds.Contains(id)))
                        ?? approvalFlows.FirstOrDefault(step => string.IsNullOrEmpty(step.Condition));

                    nextOrgPositionId = finalStep?.ToOrgPositionId;

                    if (finalStep?.IsFinal == true)
                    {
                        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    }
                }
                else
                {
                    var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(request?.OrgPositionId ?? -1);
                    nextOrgPositionId = getManagerOrgPostionId?.Id;
                }
            }

            applicationForm!.RequestStatusId = status;
            applicationForm.OrgPositionId = nextOrgPositionId;
            applicationForm.UpdatedAt = DateTimeOffset.Now;
            historyApplicationForm.Action = "Approved";

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-form-it/{itemFormIT?.Id}";

            string bodyMail = $@"
                <h4>
                    <span>Detail: </span>
                    <a href={urlApproval}>{itemFormIT?.ApplicationFormItem?.ApplicationForm?.Code}</a>
                </h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailFormIT(
                    nextUserInfo.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for IT Form approval",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> AssignedTask(AssignedTaskRequest request)
        {
            var itemITForm = await _context.ITForms
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                .Include(e => e.Priority)
                .Include(e => e.ItFormCategories)
                    .ThenInclude(e => e.ITCategory)
                .FirstOrDefaultAsync(e => e.Id == request.ITFormId || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.ITFormId))
                ?? throw new NotFoundException("IT Form not found");

            var applicationForm = itemITForm?.ApplicationFormItem?.ApplicationForm ?? throw new NotFoundException("Application form not found");

            if (request?.OrgPositionId != applicationForm.OrgPositionId)
            {
                throw new ForbiddenException(Global.NotPermissionApproval);
            }

            applicationForm.UpdatedAt = DateTimeOffset.Now;
            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.ASSIGNED; //set sang trạng thái là đẫ được gắn task
            applicationForm.OrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_AFTER_ASSIGNED_TASK; //SET 0 để không còn ai, dựa vào usercode để xử lý tiếp theo
            _context.ApplicationForms.Update(applicationForm);

            itemITForm.NoteManagerIT = request?.NoteManager;
            _context.ITForms.Update(itemITForm);

            List<AssignedTask> assignedTasks = [];
            foreach (var itemUserAssigned in request.UserAssignedTasks)
            {
                assignedTasks.Add(new AssignedTask
                {
                    ApplicationFormId = applicationForm.Id,
                    UserCode = itemUserAssigned.UserCode
                });
            }
            _context.AssignTasks.AddRange(assignedTasks);

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeAction = request.UserCodeApproval,
                ActionBy = request.UserNameApproval,
                ApplicationFormId = applicationForm.Id,
                Action = "Assigned",
                Note = request.NoteManager,
                ActionAt = DateTimeOffset.Now
            };

            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/assigned-form-it/{itemITForm.Id}";

            string bodyMail = $@"
                <h4>
                    <span>Detail: </span>
                    <a href={urlApproval}>{applicationForm?.Code}</a>
                </h4>" + TemplateEmail.EmailFormIT(itemITForm);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailFormIT(
                    request.UserAssignedTasks.Select(e => e.Email ?? "").ToList(),
                    null,
                    "New Task Assigned",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> ResolvedTask(ResolvedTaskRequest request)
        {
            var itemFormIT = await _context.ITForms
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                        .ThenInclude(af => af.AssignedTasks)
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                        .ThenInclude(af => af.HistoryApplicationForms)
                .Include(e => e.Priority)
                .Include(e => e.ItFormCategories)
                    .ThenInclude(e => e.ITCategory)
                .FirstOrDefaultAsync(e => e.Id == request.ITFormId || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.ITFormId))
                ?? throw new NotFoundException("IT Form not found, please check again");

            var applicationForm = itemFormIT?.ApplicationFormItem?.ApplicationForm
                ?? throw new NotFoundException("Application form not found, please check again");

            bool exists = applicationForm.AssignedTasks.Any(e => e.UserCode == request.UserCodeApproval);

            if (!exists)
            {
                throw new ForbiddenException(Global.NotPermissionApproval);
            }

            itemFormIT.TargetCompletionDate = request.TargetCompletionDate;
            itemFormIT.ActualCompletionDate = request.ActualCompletionDate;

            _context.ITForms.Update(itemFormIT);

            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;

            applicationForm.UpdatedAt = DateTimeOffset.Now;

            _context.ApplicationForms.Update(applicationForm);

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeAction = request.UserCodeApproval,
                ActionBy = request.UserNameApproval,
                ApplicationFormId = applicationForm.Id,
                Action = "Resolved",
                ActionAt = DateTimeOffset.Now
            };

            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            List<string> ccUserCode = [];
            ccUserCode.Add(applicationForm?.HistoryApplicationForms?.First()?.UserCodeAction ?? ""); //latest manager assigned, get usercode

            //get usercode everybody assigned task
            foreach (var itemAss in applicationForm!.AssignedTasks)
            {
                ccUserCode.Add(itemAss.UserCode ?? "");
            }

            //get email to cc, manager, user assigned task
            List<GetMultiUserViClockByOrgPositionIdResponse> multipleByUserCodes = await _userService.GetMultipleUserViclockByOrgPositionId(-1, ccUserCode);

            string urlDetail = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/view-form-it/{itemFormIT.Id}";

            string bodyMail = $@"
                <h4>
                    <span>Detail: </span>
                    <a href={urlDetail}>{itemFormIT?.ApplicationFormItem?.ApplicationForm?.Code}</a>
                </h4>" + TemplateEmail.EmailFormIT(itemFormIT);

            //send email
            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailFormIT(
                    new List<string> { itemFormIT != null ? itemFormIT.Email ?? "" : "" },
                    multipleByUserCodes.Select(e => e.Email ?? "").ToList(),
                    "Your IT form request has been approved",
                    bodyMail,
                    null,
                    true
                )
            );

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
            return query.Select(x => new Domain.Entities.ITForm
            {
                Id = x.Id,
                ApplicationFormItemId = x.ApplicationFormItemId,
                DepartmentId = x.DepartmentId,
                Email = x.Email,
                Position = x.Position,
                Reason = x.Reason,
                PriorityId = x.PriorityId,
                NoteManagerIT = x.NoteManagerIT,
                RequestDate = x.RequestDate,
                RequiredCompletionDate = x.RequiredCompletionDate,
                TargetCompletionDate = x.TargetCompletionDate,
                ActualCompletionDate = x.ActualCompletionDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Priority = x.Priority == null ? null : new Domain.Entities.Priority
                {
                    Id = x.Priority.Id,
                    Name = x.Priority.Name,
                    NameE = x.Priority.NameE
                },
                OrgUnit = x.OrgUnit == null ? null : new Domain.Entities.OrgUnit
                {
                    Id = x.OrgUnit.Id,
                    Name = x.OrgUnit.Name,
                    ParentOrgUnitId = x.OrgUnit.ParentOrgUnitId
                },
                ApplicationFormItem = x.ApplicationFormItem == null ? null : new ApplicationFormItem
                {
                    ApplicationForm = x.ApplicationFormItem.ApplicationForm == null ? null : new ApplicationForm
                    {
                        Id = x.ApplicationFormItem.ApplicationForm.Id,
                        Code = x.ApplicationFormItem.ApplicationForm.Code,
                        UserCodeCreatedBy = x.ApplicationFormItem.ApplicationForm.UserCodeCreatedBy,
                        CreatedBy = x.ApplicationFormItem.ApplicationForm.CreatedBy,

                        RequestStatusId = x.ApplicationFormItem.ApplicationForm.RequestStatusId,
                        RequestTypeId = x.ApplicationFormItem.ApplicationForm.RequestTypeId,
                        OrgPositionId = x.ApplicationFormItem.ApplicationForm.OrgPositionId,
                        CreatedAt = x.ApplicationFormItem.ApplicationForm.CreatedAt,
                        RequestStatus = x.ApplicationFormItem.ApplicationForm.RequestStatus == null ? null : new RequestStatus
                        {
                            Id = x.ApplicationFormItem.ApplicationForm.RequestStatus.Id,
                            Name = x.ApplicationFormItem.ApplicationForm.RequestStatus.Name,
                            NameE = x.ApplicationFormItem.ApplicationForm.RequestStatus.NameE,
                        },
                        RequestType = x.ApplicationFormItem.ApplicationForm.RequestType == null ? null : new Domain.Entities.RequestType
                        {
                            Id = x.ApplicationFormItem.ApplicationForm.RequestType.Id,
                            Name = x.ApplicationFormItem.ApplicationForm.RequestType.Name,
                            NameE = x.ApplicationFormItem.ApplicationForm.RequestType.NameE,
                        },
                        HistoryApplicationForms = x.ApplicationFormItem.ApplicationForm.HistoryApplicationForms.OrderByDescending(e => e.ActionAt).Select(itemHistory => new HistoryApplicationForm
                        {
                            Id = itemHistory.Id,
                            ActionBy = itemHistory.ActionBy,
                            UserCodeAction = itemHistory.UserCodeAction,
                            Action = itemHistory.Action,
                            Note = itemHistory.Note,
                            ActionAt = itemHistory.ActionAt
                        }).ToList(),
                        AssignedTasks = x.ApplicationFormItem.ApplicationForm.AssignedTasks.ToList(),
                    },
                },
                ItFormCategories = x.ItFormCategories.Select(ift => new ITFormCategory
                {
                    Id = ift.Id,
                    ITCategoryId = ift.ITCategoryId,
                    ITFormId = ift.ITFormId,
                    ITCategory = ift.ITCategory == null ? null : new ITCategory
                    {
                        Id = ift.ITCategory.Id,
                        Name = ift.ITCategory.Name,
                        Code = ift.ITCategory.Code
                    }
                }).ToList(),
            });
        }
    }
}
