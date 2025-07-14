namespace ServicePortals.Application.Interfaces.OrgUnit
{
    public interface IOrgUnitService
    {
        Task<dynamic?> GetOrgUnitById(int id);
        Task<dynamic?> GetAllDepartmentAndFirstOrgUnit();
    }
}
