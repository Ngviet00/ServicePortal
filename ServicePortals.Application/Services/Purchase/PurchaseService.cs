using System.Data;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Hangfire;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Purchase.Requests;
using ServicePortals.Application.Dtos.Purchase.Responses;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.Purchase;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;
using GroupByDepartment = ServicePortals.Application.Dtos.Purchase.Responses.GroupByDepartment;
using GroupByMonth = ServicePortals.Application.Dtos.Purchase.Responses.GroupByMonth;
using GroupByTotal = ServicePortals.Application.Dtos.Purchase.Responses.GroupByTotal;
using GroupRecentList = ServicePortals.Application.Dtos.Purchase.Responses.GroupRecentList;

namespace ServicePortals.Application.Services.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IOrgPositionService _orgPositionService;
        private readonly IConfiguration _configuration;

        public PurchaseService(
            ApplicationDbContext context,
            IOrgPositionService orgPositionService,
            IUserService userService,
            IConfiguration configuration
        )
        {
            _context = context;
            _orgPositionService = orgPositionService;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<StatisticalPurchaseResponse> StatisticalPurchase(int year)
        {
            using var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            using var multi = await connection.QueryMultipleAsync("GetPurchaseStatisticalData", new { Year = year }, commandType: CommandType.StoredProcedure);

            var result = new StatisticalPurchaseResponse
            {
                GroupByTotal = await multi.ReadFirstAsync<GroupByTotal>(),
                GroupRecentList = (await multi.ReadAsync<GroupRecentList>()).ToList(),
                GroupByDepartment = (await multi.ReadAsync<GroupByDepartment>()).ToList(),
                GroupByMonth = (await multi.ReadAsync<GroupByMonth>()).ToList()
            };

            return result;
        }

        public async Task<PagedResults<Domain.Entities.Purchase>> GetAll(GetAllPurchaseRequest request)
        {
            string? userCode = request.UserCode;
            int page = request.Page;
            int pageSize = request.PageSize;
            int? departmentId = request.DepartmentId;
            int? statusId = request.RequestStatusId;
            int? year = request.Year;

            //AsSplitQuery()
            var query = _context.Purchases.Include(e => e.ApplicationFormItem).AsQueryable();

            if (!string.IsNullOrWhiteSpace(userCode))
            {
                query = query.Where(e => e.ApplicationFormItem != null && (e.ApplicationFormItem.ApplicationForm != null && e.ApplicationFormItem.ApplicationForm.UserCodeCreatedBy == userCode));
            }

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

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var results = await SelectPurchase(query)
                .OrderByDescending(e => e.CreatedAt)
                .Skip(((page - 1) * pageSize)).Take(pageSize)
                .ToListAsync();

            return new PagedResults<Domain.Entities.Purchase>
            {
                Data = results,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<Domain.Entities.Purchase> GetById(Guid id)
        {
            var query = _context.Purchases.AsQueryable();

            var applicationFormItem = await _context.ApplicationFormItems.FirstOrDefaultAsync(e => e.ApplicationFormId == id);

            var result = await SelectPurchase(query).FirstOrDefaultAsync(e => e.Id == id || (applicationFormItem != null && e.ApplicationFormItemId == applicationFormItem.Id))
                ?? throw new ValidationException("Purchase not found");

            return result;
        }

        public async Task<object> Create(CreatePurchaseRequest request)
        {
            int? orgPositionId = request.OrgPositionId;

            if (request.DepartmentId <= 0 || request.OrgPositionId == null || request.OrgPositionId <= 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            int? nextOrgPositionId = -1;

            int status = (int)StatusApplicationFormEnum.PENDING;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.FromOrgPositionId == orgPositionId);

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
                if (orgPosition.ParentOrgPositionId == null || orgPosition.Id == Global.ParentOrgPositionGM || orgPosition.ParentOrgPositionId == Global.ParentOrgPositionGM)
                {
                    var approvalFlowsManager = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.PositonContext == "MANAGER");
                    nextOrgPositionId = approvalFlowsManager?.ToOrgPositionId;

                    if (approvalFlowsManager?.IsFinal == true)
                    {
                        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    }
                }
                else
                {
                    var orgPositionManagerOfDepartment = await _context.OrgPositions.FirstOrDefaultAsync(e => e.OrgUnitId == request.DepartmentId && e.UnitId == (int)UnitEnum.Manager);
                    nextOrgPositionId = orgPositionManagerOfDepartment?.Id;
                }
            }

            var applicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                Code = Helper.GenerateFormCode("P"),
                UserCodeCreatedBy = request.UserCode,
                CreatedBy = request.UserName,
                DepartmentId = request.DepartmentId,
                RequestTypeId = (int)RequestTypeEnum.PURCHASE,
                RequestStatusId = status,
                OrgPositionId = nextOrgPositionId,
                CreatedAt = DateTimeOffset.Now
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

            var purchase = new Domain.Entities.Purchase
            {
                Id = Guid.NewGuid(),
                ApplicationFormItemId = applicationFormItem.Id,
                DepartmentId = request?.DepartmentId,
                RequestedDate = request?.RequestedDate,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApplicationForms.Add(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);
            _context.ApplicationFormItems.Add(applicationFormItem);
            _context.Purchases.Add(purchase);

            List<PurchaseDetail> purchaseDetails = [];

            foreach (var item in request.CreatePurchaseDetailRequests)
            {
                purchaseDetails.Add(new PurchaseDetail
                {
                    RequiredDate = item.RequiredDate,
                    PurchaseId = purchase.Id,
                    ItemName = item.ItemName,
                    ItemDescription = item.ItemDescription,
                    Quantity = item.Quantity,
                    UnitMeasurement = item.UnitMeasurement,
                    CostCenterId = item.CostCenterId,
                    Note = item.Note,
                    CreatedAt = DateTimeOffset.Now
                });
            }

            _context.PurchaseDetails.AddRange(purchaseDetails);

            await _context.SaveChangesAsync();

            var purchaseById = await _context.Purchases
                .Include(e => e.OrgUnit)
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                .Include(e => e.PurchaseDetails)
                .FirstOrDefaultAsync(e => e.Id == purchase.Id);

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-purchase/{purchaseById?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Detail: </span>
                    <a href={urlApproval}>{applicationForm.Code}</a>
                </h3>" + TemplateEmail.EmailPurchase(purchaseById);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailPurchase(
                    nextUserInfo.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for purchase approval",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> Delete(Guid id)
        {
            var purchase = await _context.Purchases
                .Include(e => e.ApplicationFormItem)
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new NotFoundException("Purchase item not found");

            await _context.HistoryApplicationForms
                .Where(e => purchase.ApplicationFormItem != null && e.ApplicationFormId == purchase.ApplicationFormItem.ApplicationFormId)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationForms
                .Where(e => purchase.ApplicationFormItem != null && e.Id == purchase.ApplicationFormItem.ApplicationFormId)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.PurchaseDetails.Where(e => e.PurchaseId == purchase.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.Purchases.Where(e => e.Id == purchase.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            return true;
        }

        public async Task<object> Update(Guid id, UpdatePurchaseRequest request)
        {
            var purchase = await _context.Purchases.FirstOrDefaultAsync(e => e.Id == id || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == id)) 
                ?? throw new NotFoundException("Purchase not found");

            var requestDetailIds = request.CreatePurchaseDetailRequests.Where(r => r.Id.HasValue).Select(r => r.Id.Value).ToList();

            var existingDetails = await _context.PurchaseDetails.Where(e => e.PurchaseId == id).ToListAsync();

            var detailsToDelete = existingDetails.Where(d => !requestDetailIds.Contains(d.Id ?? Guid.Empty)).ToList();

            _context.PurchaseDetails.RemoveRange(detailsToDelete);

            foreach (var requestItem in request.CreatePurchaseDetailRequests)
            {
                if (requestItem.Id.HasValue)
                {
                    var detailToUpdate = existingDetails.FirstOrDefault(e => e.Id == requestItem.Id.Value);

                    if (detailToUpdate != null)
                    {
                        detailToUpdate.ItemName = requestItem.ItemName;
                        detailToUpdate.ItemDescription = requestItem.ItemDescription;
                        detailToUpdate.Quantity = requestItem.Quantity;
                        detailToUpdate.UnitMeasurement = requestItem.UnitMeasurement;
                        detailToUpdate.RequiredDate = requestItem.RequiredDate ?? DateTimeOffset.Now;
                        detailToUpdate.CostCenterId = requestItem.CostCenterId ?? 0;
                        detailToUpdate.Note = requestItem.Note;
                        detailToUpdate.UpdatedAt = DateTimeOffset.Now;

                        _context.PurchaseDetails.Update(detailToUpdate);
                    }
                }
                else
                {
                    var newDetail = new PurchaseDetail
                    {
                        Id = Guid.NewGuid(),
                        PurchaseId = id,
                        ItemName = requestItem.ItemName,
                        ItemDescription = requestItem.ItemDescription,
                        Quantity = requestItem.Quantity,
                        UnitMeasurement = requestItem.UnitMeasurement,
                        RequiredDate = requestItem.RequiredDate ?? DateTimeOffset.Now,
                        CostCenterId = requestItem.CostCenterId ?? 0,
                        Note = requestItem.Note,
                        CreatedAt = DateTimeOffset.Now
                    };
                    _context.PurchaseDetails.Add(newDetail);
                }
            }

            purchase.RequestedDate = request.RequestedDate;

            _context.Purchases.Update(purchase);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> Approval(ApprovalRequest request)
        {
            int? orgPositionId = request.OrgPositionId;

            if (request.OrgPositionId == null || request.OrgPositionId <= 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            var purchase = await _context.Purchases
                .Include(e => e.OrgUnit)
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                .Include(e => e.PurchaseDetails)
                .FirstOrDefaultAsync(e => e.Id == request.PurchaseId || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.PurchaseId)) ?? throw new NotFoundException("Purchase not found");

            var applicationForm = purchase?.ApplicationFormItem?.ApplicationForm ?? throw new NotFoundException("Item application form not found");

            if (request?.OrgPositionId != applicationForm.OrgPositionId)
            {
                throw new ForbiddenException(Global.NotPermissionApproval);
            }

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeAction = request.UserCodeApproval,
                ActionBy = request.UserNameApproval,
                ApplicationFormId = applicationForm.Id,
                Action = "Approved",
                ActionAt = DateTimeOffset.Now
            };

            if (request.Status != null && request.Status == false)
            {
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
                applicationForm.UpdatedAt = DateTimeOffset.Now;
                historyApplicationForm.Action = "Reject";

                _context.ApplicationForms.Update(applicationForm);
                _context.HistoryApplicationForms.Add(historyApplicationForm);

                await _context.SaveChangesAsync();

                string? reasonReject = request?.Note == null || request.Note == "" ? "--" : request?.Note;

                string bodyMailReject = $@"<h3><span style=""color:red"">Reason: {reasonReject}</span></h3>" + TemplateEmail.EmailPurchase(purchase);

                var currentUser = await _userService.GetMultipleUserViclockByOrgPositionId(-1, [applicationForm.UserCodeCreatedBy]);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailPurchase(
                        currentUser.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Your purchase request has been rejected",
                        bodyMailReject,
                        null,
                        true
                    )
                );

                return true;
            }

            int? nextOrgPositionId = 0;

            int status = (int)StatusApplicationFormEnum.IN_PROCESS;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.FromOrgPositionId == orgPosition.Id);

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
                if (orgPosition.UnitId == (int)UnitEnum.Manager || orgPosition.Id == Global.ParentOrgPositionGM || orgPosition.ParentOrgPositionId == Global.ParentOrgPositionGM)
                {
                    var approvalFlowsManager = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.PositonContext == "MANAGER");
                    nextOrgPositionId = approvalFlowsManager?.ToOrgPositionId;

                    if (approvalFlowsManager?.IsFinal == true)
                    {
                        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    }
                }
                else
                {
                    var orgPositionManagerOfDepartment = await _context.OrgPositions.FirstOrDefaultAsync(e => e.OrgUnitId == purchase.DepartmentId && e.ParentOrgPositionId == null);
                    nextOrgPositionId = orgPositionManagerOfDepartment?.Id;
                }
            }

            applicationForm.RequestStatusId = status;
            applicationForm.OrgPositionId = nextOrgPositionId;
            applicationForm.UpdatedAt = DateTimeOffset.Now;
            historyApplicationForm.Action = "APPROVAL";

            _context.ApplicationForms.Update(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-purchase/{purchase?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Detail: </span>
                    <a href={urlApproval}>{applicationForm.Code}</a>
                </h3>" + TemplateEmail.EmailPurchase(purchase);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailPurchase(
                    nextUserInfo.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for purchase approval",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> AssignedTask(AssignedTaskRequest request)
        {
            var purchase = await _context.Purchases
                .Include(e => e.OrgUnit)
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                .Include(e => e.PurchaseDetails)
                .FirstOrDefaultAsync(e => e.Id == request.PurchaseId || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.PurchaseId)) ?? throw new NotFoundException("Purchase not found");

            var applicationForm = purchase?.ApplicationFormItem?.ApplicationForm ?? throw new NotFoundException("Application form not found");

            if (request?.OrgPositionId != applicationForm.OrgPositionId)
            {
                throw new ForbiddenException(Global.NotPermissionApproval);
            }

            applicationForm.UpdatedAt = DateTimeOffset.Now;
            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.ASSIGNED; //set sang trạng thái là đẫ được gắn task
            applicationForm.OrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_AFTER_ASSIGNED_TASK; //SET 0 để không còn ai, dựa vào usercode để xử lý tiếp theo
            _context.ApplicationForms.Update(applicationForm);

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
                Note = request.Note,
                ActionAt = DateTimeOffset.Now
            };

            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlAssigned = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/assigned-purchase/{purchase?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Detail: </span>
                    <a href={urlAssigned}>{purchase?.ApplicationFormItem?.ApplicationForm?.Code}</a>
                </h3>" + TemplateEmail.EmailPurchase(purchase);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailPurchase(
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
            var purchase = await _context.Purchases
                .Include(e => e.OrgUnit)
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                        .ThenInclude(af => af.AssignedTasks)
                .Include(e => e.ApplicationFormItem)
                    .ThenInclude(e => e.ApplicationForm)
                        .ThenInclude(af => af.HistoryApplicationForms)
                .Include(e => e.PurchaseDetails)
                .FirstOrDefaultAsync(e => e.Id == request.PurchaseId ||  (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.PurchaseId))
                ?? throw new NotFoundException("Purchase not found");

            var applicationForm = purchase?.ApplicationFormItem?.ApplicationForm
                ?? throw new NotFoundException("Not found data, please check again");

            bool exists = applicationForm.AssignedTasks.Any(e => e.UserCode == request.UserCodeApproval);

            if (!exists)
            {
                throw new ForbiddenException(Global.NotPermissionApproval);
            }

            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
            applicationForm.UpdatedAt = DateTimeOffset.Now;
            _context.ApplicationForms.Update(applicationForm);

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeAction = request.UserCodeApproval,
                ActionBy = request.UserNameApproval,
                ApplicationFormId = applicationForm.Id,
                Action = "Resolved",
                Note = request.Note,
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

            var userSendRequestPurchase = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == purchase.ApplicationFormItem.ApplicationForm.UserCodeCreatedBy);

            var emailUserSendRequestPurchase = userSendRequestPurchase?.Email ?? "";

            //get email to cc, manager, user assigned task
            List<GetMultiUserViClockByOrgPositionIdResponse> multipleByUserCodes = await _userService.GetMultipleUserViclockByOrgPositionId(-1, ccUserCode);

            string urlDetail = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/view-purchase/{purchase?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Detail: </span>
                    <a href={urlDetail}>{purchase?.ApplicationFormItem.ApplicationForm?.Code}</a>
                </h3>" + TemplateEmail.EmailPurchase(purchase);

            //send email
            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailPurchase(
                    new List<string> { emailUserSendRequestPurchase },
                    multipleByUserCodes.Select(e => e.Email ?? "").ToList(),
                    "Your purchase request has been approved",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<List<InfoUserAssigned>> GetMemberPurchaseAssigned()
        {
            var results = await _context.Database.GetDbConnection()
                .QueryAsync<InfoUserAssigned>(
                    "dbo.Purchasing_GET_GetListMemberAssigned",
                    commandType: CommandType.StoredProcedure
            );

            return (List<InfoUserAssigned>)results;
        }

        private static IQueryable<Domain.Entities.Purchase> SelectPurchase(IQueryable<Domain.Entities.Purchase> query)
        {
            return query.Select(x => new Domain.Entities.Purchase
            {
                Id = x.Id,
                ApplicationFormItemId = x.ApplicationFormItemId,
                DepartmentId = x.DepartmentId,
                RequestedDate = x.RequestedDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                OrgUnit = x.OrgUnit == null ? null : new Domain.Entities.OrgUnit
                {
                    Id = x.OrgUnit.Id,
                    UnitId = x.OrgUnit.UnitId,
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
                PurchaseDetails = x.PurchaseDetails != null ? x.PurchaseDetails.OrderByDescending(h => h.CreatedAt).ToList() : new List<PurchaseDetail> { }
            });
        }
    }
}
