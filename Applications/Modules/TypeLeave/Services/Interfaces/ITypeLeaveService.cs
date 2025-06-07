using ServicePortal.Applications.Modules.TypeLeave.DTO;
using ServicePortal.Applications.Modules.TypeLeave.DTO.Requests;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.TypeLeave.Services.Interfaces
{
    public interface ITypeLeaveService
    {
        Task<List<Domain.Entities.TypeLeave>> GetAll(GetAllTypeLeaveRequestDto request);
        Task<Domain.Entities.TypeLeave> GetById(int id);
        Task<Domain.Entities.TypeLeave> Create(TypeLeaveDto dto);
        Task<Domain.Entities.TypeLeave> Update(int id, TypeLeaveDto dto);
        Task<Domain.Entities.TypeLeave> Delete(int id);
    }
}
