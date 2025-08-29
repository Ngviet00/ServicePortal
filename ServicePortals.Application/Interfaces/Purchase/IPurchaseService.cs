using ServicePortals.Application.Dtos.Purchase.Requests;
using ServicePortals.Application.Dtos.Purchase.Responses;

namespace ServicePortals.Application.Interfaces.Purchase
{
    public interface IPurchaseService
    {
        Task<PagedResults<PurchaseResponse>> GetAll(GetAllPurchaseRequest request);
        Task<PurchaseResponse> GetById(Guid id, bool? isHasRelationApplication = null);
        Task<object> Create(CreatePurchaseRequest request);
        Task<object> Update(Guid id, UpdatePurchaseRequest request);
        Task<object> Delete(Guid id);
        Task<object> DeletePurchaseItem(Guid purchaseItemId); //force delete each purchase item
        //Task<object> Approval();
        //Task<object> AssignedTask();
        //Task<object> ResolvedTask();
    }
}
