using ServicePortal.Domain.Entities;
using ServicePortal.Modules.Position.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class PositionMapper
    {
        public static PositionDTO ToDto(Position entity)
        {
            return new PositionDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                PositionLevel = entity.PositionLevel,
            };
        }

        public static Position ToEntity(PositionDTO dto)
        {
            return new Position
            {
                Name = dto.Name,
                PositionLevel = dto.PositionLevel
            };
        }

        public static List<PositionDTO> ToDtoList(List<Position> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        public static List<Position> ToEntityList(List<PositionDTO> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
