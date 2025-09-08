using System.Linq.Expressions;
using ServicePortals.Application.Dtos.OrgUnit.Requests;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.OrgUnit
{
    public interface IOrgUnitService
    {
        Task<List<Entities.OrgUnit>> GetAllDepartments();
        Task<object> GetTeamByDeptIdAndUserNotSetOrgPositionId(int departmentId);
        Task<object> GetListUserByTeamId(int teamId);
        Task<bool> SaveChangeUserOrgUnit(SaveChangeOrgUnitUserRequest request);
        Task<dynamic?> GetDepartmentAndChildrenTeam();
        Task<List<Entities.OrgUnit>> GetAll(Expression<Func<Entities.OrgUnit, bool>>? predicate = null, int? departmentId = null);
    }
}
