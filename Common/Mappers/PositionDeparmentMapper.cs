using ServicePortal.Domain.Entities;
using ServicePortal.Modules.PositionDeparment.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class PositionDeparmentMapper
    {
        public static PositionDeparmentDTO ToDto(PositionDeparment entity)
        {
            return new PositionDeparmentDTO
            {
                Id = entity.Id,
                DeparmentId = entity.DeparmentId,
                PositionId = entity.PositionId,
                PositionDeparmentLevel = entity.PositionDeparmentLevel,
                CustomTitle = entity.CustomTitle,
            };
        }

        public static PositionDeparment ToEntity(PositionDeparmentDTO dto)
        {
            return new PositionDeparment
            {
                DeparmentId = dto.DeparmentId,
                PositionId = dto.PositionId,
                PositionDeparmentLevel = dto.PositionDeparmentLevel,
                CustomTitle = dto.CustomTitle,
            };
        }
    }
}
