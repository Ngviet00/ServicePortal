namespace ServicePortals.Application.Dtos.OrgUnit
{
    public class OrgUnitDto
    {
        public int? Id { get; set; }
        public int? DeptId { get; set; }
        public string? Name { get; set; }
        public int? UnitId { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int? ParentJobTitleId { get; set; }
        public string? ManagerUserCode { get; set; }
        public string? DeputyUserCode { get; set; }
    }
}
