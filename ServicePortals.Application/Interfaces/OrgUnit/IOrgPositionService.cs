using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.OrgUnit
{
    public interface IOrgPositionService
    {
        Task<List<OrgPosition>> GetOrgPositionsByDepartmentId(int? departmentId);
        Task<OrgPosition?> GetManagerOrgPostionIdByOrgPositionId(int orgPostionId);
    }
}
