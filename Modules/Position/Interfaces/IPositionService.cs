using ServicePortal.Common;
using ServicePortal.Modules.Position.DTO;
using ServicePortal.Modules.Position.Requests;

namespace ServicePortal.Modules.Position.Interfaces
{
    public interface IPositionService
    {
        Task<PagedResults<PositionDTO>> GetAll(GetAllPositionRequest request);
        Task<PositionDTO> GetById(int id);
        Task<PositionDTO> Create(PositionDTO dto);
        Task<PositionDTO> Update(int id, PositionDTO dto);
        Task<PositionDTO> Delete(int id);
        Task<PositionDTO> ForceDelete(int id);
    }
}
