using ServicePortal.Domain.Entities;
using ServicePortal.Modules.Team.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class TeamMapper
    {
        public static TeamDTO ToDto(Team entity)
        {
            return new TeamDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                DepartmentId = entity.DepartmentId
            };
        }

        public static Team ToEntity(TeamDTO dto)
        {
            return new Team
            {
                Name = dto.Name,
                DepartmentId = dto.DepartmentId
            };
        }

        public static List<TeamDTO> ToDtoList(List<Team> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        public static List<Team> ToEntityList(List<TeamDTO> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
