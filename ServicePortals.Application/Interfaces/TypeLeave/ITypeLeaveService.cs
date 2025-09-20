using ServicePortals.Application.Dtos.TypeLeave;
using ServicePortals.Application.Dtos.TypeLeave.Requests;

namespace ServicePortals.Application.Interfaces.TypeLeave
{
    public interface ITypeLeaveService
    {
        Task<List<Domain.Entities.TypeLeave>> GetAll(GetAllTypeLeaveRequest request);
        Task<Domain.Entities.TypeLeave> GetById(int id);
        Task<Domain.Entities.TypeLeave> Create(TypeLeaveDto request);
        Task<Domain.Entities.TypeLeave> Update(int id, TypeLeaveDto request);
        Task<Domain.Entities.TypeLeave> Delete(int id);
    }
}
