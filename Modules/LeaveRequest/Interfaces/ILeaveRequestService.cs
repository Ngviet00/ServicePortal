using ServicePortal.Common;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.Requests;

namespace ServicePortal.Modules.LeaveRequest.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<PagedResults<LeaveRequestDTO>> GetAll(GetAllLeaveRequest request);
        Task<LeaveRequestDTO> GetById(Guid id);
        Task<LeaveRequestDTO> Create(LeaveRequestDTO dto);
        Task<LeaveRequestDTO> Update(Guid id, LeaveRequestDTO dto);
        Task<LeaveRequestDTO> Delete(Guid id);
    }
}
