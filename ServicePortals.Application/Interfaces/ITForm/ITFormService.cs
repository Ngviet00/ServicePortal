using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Dtos.ITForm.Responses;

namespace ServicePortals.Application.Interfaces.ITForm
{
    public interface ITFormService
    {
        Task<PagedResults<ITFormResponse>> GetAll(GetAllITFormRequest request);
        Task<ITFormResponse?> GetById(Guid Id);
        Task<object> Create(CreateITFormRequest request);
        Task<object> Update(Guid Id, UpdateITFormRequest request);
        Task<object> Delete(Guid Id);
        Task<object> Approval(ApprovalRequest request);
    }
}
