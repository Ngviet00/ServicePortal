using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Dtos.ITForm.Responses;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Shared.SharedDto.Requests;

namespace ServicePortals.Application.Interfaces.ITForm
{
    public interface ITFormService
    {
        Task<StatisticalFormITResponse> StatisticalFormIT(int year);
        Task<PagedResults<GetListITFormResponse>> GetAll(GetAllITFormRequest request);
        Task<Domain.Entities.ITForm?> GetById(Guid Id);
        Task<object> Create(CreateITFormRequest request);
        Task<object> Update(Guid Id, UpdateITFormRequest request);
        Task<object> Delete(Guid Id);
        Task<object> Approval(ApprovalRequest request);
        Task<object> AssignedTask(AssignedTaskRequest request);
        Task<object> ResolvedTask(ResolvedTaskRequest request);
        Task<List<InfoUserAssigned>> GetMemberITAssigned();
    }
}
