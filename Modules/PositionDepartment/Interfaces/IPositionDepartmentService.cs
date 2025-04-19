using ServicePortal.Modules.PositionDeparment.DTO;

namespace ServicePortal.Modules.PositionDeparment.Interfaces
{
    public interface IPositionDepartmentService
    {
        Task<PositionDepartmentDTO> Create(PositionDepartmentDTO dto);
    }
}
