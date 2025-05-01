using ServicePortal.Common;
using ServicePortal.Modules.TypeLeave.DTO;
using ServicePortal.Modules.TypeLeave.DTO.Requests;

namespace ServicePortal.Modules.TypeLeave.Interfaces
{
    public interface ITypeLeaveService
    {
        Task<PagedResults<Domain.Entities.TypeLeave>> GetAll(GetAllTypeLeaveRequest request);
        Task<Domain.Entities.TypeLeave> GetById(int id);
        Task<Domain.Entities.TypeLeave> Create(TypeLeaveDTO dto);
        Task<Domain.Entities.TypeLeave> Update(int id, TypeLeaveDTO dto);
        Task<Domain.Entities.TypeLeave> Delete(int id);
    }
}
