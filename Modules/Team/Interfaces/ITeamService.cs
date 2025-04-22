using ServicePortal.Common;
using ServicePortal.Modules.Team.DTO;
using ServicePortal.Modules.Team.Requests;

namespace ServicePortal.Modules.Team.Interfaces
{
    public interface ITeamService
    {
        Task<PagedResults<TeamDTO>> GetAll(GetAllTeamRequest request);
        Task<Domain.Entities.Team> GetById(int id);
        Task<Domain.Entities.Team> Create(TeamDTO dto);
        Task<Domain.Entities.Team> Update(int id, TeamDTO dto);
        Task<Domain.Entities.Team> Delete(int id);
    }
}
