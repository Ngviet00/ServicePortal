using ServicePortal.Common;
using ServicePortal.Modules.CustomApprovalFlow.DTO;

namespace ServicePortal.Modules.CustomApprovalFlow.Interfaces
{
    public interface ICustomApprovalFlowService
    {
        Task<PagedResults<CustomApprovalFlowDTO>> GetAll(CustomApprovalFlowDTO dto);
        Task<CustomApprovalFlowDTO> GetById(int id);
        Task<Domain.Entities.CustomApprovalFlow> Create(CustomApprovalFlowDTO dto);
        Task<Domain.Entities.CustomApprovalFlow> Update(int id, CustomApprovalFlowDTO dto);
        Task<Domain.Entities.CustomApprovalFlow> Delete(int id);
    }
}
