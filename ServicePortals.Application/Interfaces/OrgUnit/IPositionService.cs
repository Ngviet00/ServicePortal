using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.OrgUnit
{
    public interface IPositionService
    {
        Task<List<Position>> GetPositionsByDepartmentId(int departmentId);
    }
}
