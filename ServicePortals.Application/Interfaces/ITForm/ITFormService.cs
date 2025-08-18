using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;

namespace ServicePortals.Application.Interfaces.ITForm
{
    public interface ITFormService
    {
        Task<object> GetAll(GetAllITFormRequest request);
        Task<object> GetById(Guid Id);
        Task<object> Create(CreateITFormRequest request);
        Task<object> Update(Guid Id, UpdateITFormRequest request);
        Task<object> Delete(Guid Id);
        Task<object> Approval(ApprovalRequest request);
    }
}
