using ServicePortal.Domain.Entities;
using ServicePortal.Modules.PositionDeparment.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class PositionDepartmentMapper
    {
        public static PositionDepartmentDTO ToDto(PositionDepartment entity)
        {
            return new PositionDepartmentDTO
            {
                Id = entity.Id,
                DeparmentId = entity.DeparmentId,
                PositionId = entity.PositionId,
                PositionDeparmentLevel = entity.PositionDeparmentLevel,
                CustomTitle = entity.CustomTitle,
            };
        }

        public static PositionDepartment ToEntity(PositionDepartmentDTO dto)
        {
            return new PositionDepartment
            {
                DeparmentId = dto.DeparmentId,
                PositionId = dto.PositionId,
                PositionDeparmentLevel = dto.PositionDeparmentLevel,
                CustomTitle = dto.CustomTitle,
            };
        }
    }
}
