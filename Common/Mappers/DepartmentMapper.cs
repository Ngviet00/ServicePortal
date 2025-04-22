using ServicePortal.Domain.Entities;
using ServicePortal.Modules.Deparment.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class DepartmentMapper
    {
        public static DepartmentDTO ToDto(Department entity)
        {
            return new DepartmentDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Note = entity.Note,
                ParentId = entity.ParentId,
            };
        }

        public static Department ToEntity(DepartmentDTO dto)
        {
            return new Department
            {
                Name = dto.Name,
                Note = dto.Note,
                ParentId = dto.ParentId,
            };
        }

        public static List<DepartmentDTO> ToDtoList(List<Department> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        public static List<Department> ToEntityList(List<DepartmentDTO> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
