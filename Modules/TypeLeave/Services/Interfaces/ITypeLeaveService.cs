using ServicePortal.Common;
using ServicePortal.Modules.TypeLeave.DTO;
using ServicePortal.Modules.TypeLeave.DTO.Requests;

namespace ServicePortal.Modules.TypeLeave.Services.Interfaces
{
    public interface ITypeLeaveService
    {
        Task<PagedResults<Domain.Entities.TypeLeave>> GetAll(GetAllTypeLeaveRequestDto request);
        Task<Domain.Entities.TypeLeave> GetById(int id);
        Task<Domain.Entities.TypeLeave> Create(TypeLeaveDto dto);
        Task<Domain.Entities.TypeLeave> Update(int id, TypeLeaveDto dto);
        Task<Domain.Entities.TypeLeave> Delete(int id);
    }
}
