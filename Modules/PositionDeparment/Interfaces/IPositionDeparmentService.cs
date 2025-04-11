using ServicePortal.Modules.PositionDeparment.DTO;

namespace ServicePortal.Modules.PositionDeparment.Interfaces
{
    public interface IPositionDeparmentService
    {
        Task<PositionDeparmentDTO> Create(PositionDeparmentDTO dto);
    }
}
