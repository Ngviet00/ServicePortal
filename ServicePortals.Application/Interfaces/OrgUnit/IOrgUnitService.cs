using ServicePortals.Application.Dtos.OrgUnit;
using ServicePortals.Application.Dtos.OrgUnit.Requests;
using ServicePortals.Shared.SharedDto;
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
        //Task<List<TreeCheckboxResponse>> GetAllDeptOfOrgUnit();
        //Task<Domain.Entities.OrgUnit?> GetOrgUnitById(int id);
        //Task<List<OrgUnitDto>> GetOrgUnitUserWithDept(int departmentId);

    }
}
