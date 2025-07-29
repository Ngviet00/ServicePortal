namespace ServicePortals.Application.Interfaces.OrgUnit
{
    public interface IOrgUnitService
    {
        Task<Domain.Entities.OrgUnit?> GetOrgUnitById(int id);
        Task<dynamic?> GetAllDepartmentAndFirstOrgUnit();
        Task<dynamic?> GetOrgUnitUserWithDepartment();
    }
}
