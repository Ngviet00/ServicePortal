using ServicePortal.Modules.Deparment.DTO;

namespace ServicePortal.Modules.Deparment.Interfaces
{
    public interface IDeparmentService
    {
        Task<List<Domain.Entities.Deparment>> GetAll();
        Task<Domain.Entities.Deparment> GetById(int id);
        Task<Domain.Entities.Deparment> Create(DeparmentDTO dto);
        Task<Domain.Entities.Deparment> Update(int id, DeparmentDTO dto);
        Task<Domain.Entities.Deparment> Delete(int id);
        Task<Domain.Entities.Deparment> ForceDelete(int id);
    }
}
