using ServicePortal.Common;
using ServicePortal.Modules.CustomApprovalFlow.DTO;

namespace ServicePortal.Modules.CustomApprovalFlow.Services.Interfaces
{
    public interface ICustomApprovalFlowService
    {
        Task<PagedResults<CustomApprovalFlowDto>> GetAll(CustomApprovalFlowDto dto);
        Task<CustomApprovalFlowDto> GetById(int id);
        Task<Domain.Entities.CustomApprovalFlow> Create(CustomApprovalFlowDto dto);
        Task<Domain.Entities.CustomApprovalFlow> Update(int id, CustomApprovalFlowDto dto);
        Task<Domain.Entities.CustomApprovalFlow> Delete(int id);
    }
}
