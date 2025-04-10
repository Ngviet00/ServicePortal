using ServicePortal.Domain.Entities;
using ServicePortal.Modules.Deparment.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class DeparmentMapper
    {
        public static DeparmentDTO ToDto(Deparment entity)
        {
            return new DeparmentDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Note = entity.Note,
                ParentId = entity.ParentId,
            };
        }

        public static Deparment toEntity(DeparmentDTO dto)
        {
            return new Deparment
            {
                Name = dto.Name,
                Note = dto.Note,
                ParentId = dto.ParentId,
            };
        }
    }
}
