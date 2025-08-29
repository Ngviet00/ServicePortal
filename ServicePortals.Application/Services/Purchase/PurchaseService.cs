using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Purchase.Requests;
using ServicePortals.Application.Dtos.Purchase.Responses;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.Purchase;
using ServicePortals.Application.Mappers;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.Purchase
{
    public class PurchaseService : IPurchaseService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrgPositionService _orgPositionService;

        public PurchaseService(ApplicationDbContext context, IOrgPositionService orgPositionService)
        {
            _context = context;
            _orgPositionService = orgPositionService;
        }

        public Task<PagedResults<PurchaseResponse>> GetAll(GetAllPurchaseRequest request)
        {
            throw new NotImplementedException();
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

        public async Task<object> DeletePurchaseItem(Guid purchaseItemId)
        {
            await _context.PurchaseDetails.Where(e => e.Id == purchaseItemId).ExecuteDeleteAsync();

            return true;
        }

        public Task<object> Update(Guid id, UpdatePurchaseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
