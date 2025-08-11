using ServicePortals.Application.Dtos.OrgUnit;
using ServicePortals.Application.Dtos.OrgUnit.Requests;
using ServicePortals.Shared.SharedDto;

namespace ServicePortals.Application.Interfaces.OrgUnit
{
    public interface IOrgUnitService
    {
        Task<List<OrgUnitDto>> GetAllDepartments();
        Task<List<TreeCheckboxResponse>> GetAllDeptOfOrgUnit();
        Task<Domain.Entities.OrgUnit?> GetOrgUnitById(int id);
        Task<dynamic?> GetAllDepartmentAndFirstOrgUnit();
        Task<object> GetOrgUnitTeamAndUserNotSetOrgUnitWithDept(int departmentId);
        Task<List<OrgUnitDto>> GetOrgUnitUserWithDept(int departmentId);
        Task<bool> SaveChangeUserOrgUnit(SaveChangeOrgUnitUserRequest request);
    }
}
