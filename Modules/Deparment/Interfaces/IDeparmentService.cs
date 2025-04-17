using ServicePortal.Common;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Requests;

namespace ServicePortal.Modules.Deparment.Interfaces
{
    public interface IDeparmentService
    {
        Task<PagedResults<Domain.Entities.Deparment>> GetAll(GetAllDeparmentRequest request);
        Task<Domain.Entities.Deparment> GetById(int id);
        Task<Domain.Entities.Deparment> Create(DeparmentDTO dto);
        Task<Domain.Entities.Deparment> Update(int id, DeparmentDTO dto);
        Task<Domain.Entities.Deparment> Delete(int id);
    }
}
