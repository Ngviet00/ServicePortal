using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Purchase.Requests;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;

namespace ServicePortals.Application.Interfaces.Purchase
{
    public interface IPurchaseService
    {
        Task<PagedResults<Domain.Entities.Purchase>> GetAll(GetAllPurchaseRequest request);
        Task<Domain.Entities.Purchase> GetById(Guid id);
        Task<object> Create(CreatePurchaseRequest request);
        Task<object> Update(Guid id, UpdatePurchaseRequest request);
        Task<object> Delete(Guid id);
        Task<object> Approval(ApprovalRequest request);
        Task<object> AssignedTask(AssignedTaskRequest request);
        Task<object> ResolvedTask(ResolvedTaskRequest request);
        Task<List<InfoUserAssigned>> GetMemberPurchaseAssigned();
    }
}
