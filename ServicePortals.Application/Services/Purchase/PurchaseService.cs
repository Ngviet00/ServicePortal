using System.Data;
using Dapper;
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
using ServicePortals.Application.Mappers;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;

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

        public async Task<PagedResults<PurchaseResponse>> GetAll(GetAllPurchaseRequest request)
        {
            string? userCode = request.UserCode;
            int page = request.Page;
            int pageSize = request.PageSize;
            int? departmentId = request.DepartmentId;
            int? statusId = request.RequestStatusId;
            int? year = request.Year;

            var query = _context.Purchases.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userCode))
            {
                query = query.Where(e => e.UserCode == userCode);
            }

            if (departmentId != null)
            {
                query = query.Where(e => e.DepartmentId == departmentId);
            }

            if (year != null)
            {
                query = query.Where(e => e.CreatedAt != null && e.CreatedAt.Value.Year == year);
            }

            if (statusId != null)
            {
                if (statusId == (int)StatusApplicationFormEnum.PENDING || statusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL)
                {
                    query = query.Where(e => e.ApplicationForm != null &&
                        (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.PENDING ||
                         e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL));
                }
                else if (statusId == (int)StatusApplicationFormEnum.IN_PROCESS || statusId == (int)StatusApplicationFormEnum.ASSIGNED)
                {
                    query = query.Where(e => e.ApplicationForm != null &&
                        (e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.IN_PROCESS ||
                         e.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.ASSIGNED));
                }
                else
                {
                    query = query.Where(e => e.ApplicationForm != null &&
                        e.ApplicationForm.RequestStatusId == statusId);
                }
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var result = await query
                .OrderByDescending(e => e.CreatedAt)
                .Include(e => e.OrgUnit)
                .Include(e => e.PurchaseDetails)
                .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.HistoryApplicationForms)
                .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.RequestStatus)
                .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.RequestType)
                .Include(e => e.ApplicationForm)
                .Skip(((page - 1) * pageSize)).Take(pageSize)
                .ToListAsync();

            var resultDtos = PurchaseMapper.ToDtoList(result);

            return new PagedResults<PurchaseResponse>
            {
                Data = resultDtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<PurchaseResponse> GetById(Guid id, bool? isHasRelationApplication = null)
        {
            var query = _context.Purchases.Include(e => e.OrgUnit).Include(e => e.PurchaseDetails).AsQueryable();

            if (isHasRelationApplication != null && isHasRelationApplication == true)
            {
                query = query
                 .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.HistoryApplicationForms)
                 .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.RequestStatus)
                 .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.RequestType)
                 .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.AssignedTasks);
            }

            var item = await query.FirstOrDefaultAsync(e => e.Id == id) ?? throw new ValidationException("Purchase not found");

            return PurchaseMapper.ToDto(item);
        }

        public async Task<object> Create(CreatePurchaseRequest request)
        {
            int? orgPositionId = request.OrgPositionId;

            if (request.DepartmentId <= 0 || request.OrgPositionId == null || request.OrgPositionId <= 0)
            {
                throw new ValidationException("Thông tin vị trí người dùng chưa được cập nhật, liên hệ với HR");
            }

            int? nextOrgPositionId = -1;

            int status = (int)StatusApplicationFormEnum.PENDING;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId);

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.FromOrgPositionId == orgPositionId);

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
                if (orgPosition?.ParentOrgPositionId == null) //manager các bộ phận
                {
                    var approvalFlows = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.PositonContext == "MANAGER");
                    nextOrgPositionId = approvalFlows?.ToOrgPositionId;
                }
                else //nhân viên
                {
                    var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(orgPositionId ?? -1);
                    nextOrgPositionId = getManagerOrgPostionId?.Id;
                }
            }

            var applicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                UserCodeRequestor = request.UserCode,
                UserNameRequestor = request.UserName,
                RequestTypeId = (int)RequestTypeEnum.PURCHASE,
                RequestStatusId = status,
                OrgPositionId = nextOrgPositionId,
                CreatedAt = DateTimeOffset.Now
            };

            _context.ApplicationForms.Add(applicationForm);

            var purchase = new Domain.Entities.Purchase
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                Code = Helper.GenerateFormCode("P"),
                UserCode = request.UserCode,
                UserName = request.UserName,
                DepartmentId = request.DepartmentId,
                RequestedDate = request.RequestedDate,
                CreatedAt = DateTimeOffset.Now
            };

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

            var purchaseById = await _context.Purchases.Include(e => e.OrgUnit).Include(e => e.PurchaseDetails).FirstOrDefaultAsync(e => e.Id == purchase.Id);

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-purchase/{purchaseById?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Click to detail: </span>
                    <a href={urlApproval}>{purchase.Code}</a>
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
            var purchase = await _context.Purchases.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Purchase item not found");

            await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == purchase.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationForms.Where(e => e.Id == purchase.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.PurchaseDetails.Where(e => e.PurchaseId == purchase.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.Purchases.Where(e => e.Id == purchase.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            return true;
        }

        public async Task<object> Update(Guid id, UpdatePurchaseRequest request)
        {
            var purchase = await _context.Purchases.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Purchase not found");

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
                throw new ValidationException("User information is not updated, contact HR");
            }

            var purchase = await _context.Purchases.Include(e => e.OrgUnit).Include(e => e.PurchaseDetails).FirstOrDefaultAsync(e => e.Id == request.PurchaseId) ?? throw new NotFoundException("Purchase not found");

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == purchase.ApplicationFormId) ?? throw new NotFoundException("Item application form not found");

            if (request?.OrgPositionId != applicationForm.OrgPositionId)
            {
                throw new ForbiddenException("You are not permitted to approve this request.");
            }

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeApproval = request?.UserCodeApproval,
                UserNameApproval = request?.UserNameApproval,
                ApplicationFormId = applicationForm?.Id,
                Note = request?.Note,
                CreatedAt = DateTimeOffset.Now
            };

            if (request.Status != null && request.Status == false)
            {
                applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
                applicationForm.UpdatedAt = DateTimeOffset.Now;
                historyApplicationForm.Action = "REJECT";

                _context.ApplicationForms.Update(applicationForm);
                _context.HistoryApplicationForms.Add(historyApplicationForm);

                await _context.SaveChangesAsync();

                string? reasonReject = request?.Note == null || request.Note == "" ? "--" : request?.Note;

                string bodyMailReject = $@"<h3><span style=""color:red"">Reason: {reasonReject}</span></h3>" + TemplateEmail.EmailPurchase(purchase);

                var currentUser = await _userService.GetMultipleUserViclockByOrgPositionId(-1, [purchase.UserCode]);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.SendEmailPurchase(
                        currentUser.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Request purchase has been reject",
                        bodyMailReject,
                        null,
                        true
                    )
                );

                return true;
            }

            int? nextOrgPositionId = 0;

            int status = (int)StatusApplicationFormEnum.IN_PROCESS;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException("Bạn chưa được cập nhật vị trí, liên hệ với HR");

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
                if (orgPosition?.ParentOrgPositionId == null) //manager các bộ phận
                {
                    var approvalFlows = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.PURCHASE && e.PositonContext == "MANAGER");
                    nextOrgPositionId = approvalFlows?.ToOrgPositionId;
                }
                else //nhân viên
                {
                    var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(orgPositionId ?? -1);
                    nextOrgPositionId = getManagerOrgPostionId?.Id;
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
                    <span>Click to detail: </span>
                    <a href={urlApproval}>{purchase.Code}</a>
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
            var purchase = await _context.Purchases.Include(e => e.OrgUnit).Include(e => e.PurchaseDetails).FirstOrDefaultAsync(e => e.Id == request.PurchaseId) ?? throw new NotFoundException("Purchase not found");

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => e.Id == purchase.ApplicationFormId) ?? throw new NotFoundException("Item application form not found");

            if (request?.OrgPositionId != applicationForm.OrgPositionId)
            {
                throw new ForbiddenException("You are not permitted to approve this request.");
            }

            applicationForm.UpdatedAt = DateTimeOffset.Now;
            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.ASSIGNED; //set sang trạng thái là đẫ được gắn task
            applicationForm.OrgPositionId = (int)StatusApplicationFormEnum.ORG_POSITION_ID_AFTER_ASSIGNED_TASK; //SET -1 để không còn ai, dựa vào usercode để xử lý tiếp theo
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
                UserCodeApproval = request.UserCodeApproval,
                UserNameApproval = request.UserNameApproval,
                ApplicationFormId = applicationForm.Id,
                Action = "APPROVAL",
                Note = request.NoteManager,
                CreatedAt = DateTimeOffset.Now
            };

            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            string urlAssigned = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/assigned-purchase/{purchase?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Click to detail: </span>
                    <a href={urlAssigned}>{purchase?.Code}</a>
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
            var purchase = await _context.Purchases.Include(e => e.OrgUnit).Include(e => e.PurchaseDetails).FirstOrDefaultAsync(e => e.Id == request.PurchaseId) ?? throw new NotFoundException("Purchase not found");

            var applicationForm = await _context.ApplicationForms
                .Include(e => e.HistoryApplicationForms)
                .Include(e => e.AssignedTasks)
                .FirstOrDefaultAsync(e => e.Id == purchase.ApplicationFormId)
                ?? throw new NotFoundException("Not found data, please check again");

            bool exists = applicationForm.AssignedTasks.Any(e => e.UserCode == request.UserCodeApproval);

            if (!exists)
            {
                throw new ForbiddenException("You are not permitted to approve this request.");
            }

            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
            applicationForm.UpdatedAt = DateTimeOffset.Now;
            _context.ApplicationForms.Update(applicationForm);

            var historyApplicationForm = new HistoryApplicationForm
            {
                UserCodeApproval = request.UserCodeApproval,
                UserNameApproval = request.UserNameApproval,
                ApplicationFormId = applicationForm.Id,
                Action = "RESOLVED",
                CreatedAt = DateTimeOffset.Now
            };

            _context.HistoryApplicationForms.Add(historyApplicationForm);

            await _context.SaveChangesAsync();

            List<string> ccUserCode = [];
            ccUserCode.Add(applicationForm?.HistoryApplicationForms?.First()?.UserCodeApproval ?? ""); //latest manager assigned, get usercode

            //get usercode everybody assigned task
            foreach (var itemAss in applicationForm!.AssignedTasks)
            {
                ccUserCode.Add(itemAss.UserCode ?? "");
            }

            var userSendRequestPurchase = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == purchase.UserCode);

            var emailUserSendRequestPurchase = userSendRequestPurchase?.Email ?? "";

            //get email to cc, manager, user assigned task
            List<GetMultiUserViClockByOrgPositionIdResponse> multipleByUserCodes = await _userService.GetMultipleUserViclockByOrgPositionId(-1, ccUserCode);

            string urlDetail = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/view-purchase/{purchase?.Id}";

            string bodyMail = $@"
                <h3>
                    <span>Click to detail: </span>
                    <a href={urlDetail}>{purchase?.Code}</a>
                </h3>" + TemplateEmail.EmailPurchase(purchase);

            //send email
            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailPurchase(
                    new List<string> { emailUserSendRequestPurchase },
                    multipleByUserCodes.Select(e => e.Email ?? "").ToList(),
                    "Your purchase request has been successfully processed",
                    bodyMail,
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<List<InfoUserAssigned>> GetMemberPurchaseAssigned()
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
                    NV.NVNgayRa > GETDATE() AND NV.ViTriToChucId IN (30, 31)
            ";

            var result = await connection.QueryAsync<InfoUserAssigned>(sql);

            return (List<InfoUserAssigned>)result;
        }
    }
}
