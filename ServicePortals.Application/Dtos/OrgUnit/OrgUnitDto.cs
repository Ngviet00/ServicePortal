namespace ServicePortals.Application.Dtos.OrgUnit
{
    public class OrgUnitDto
    {
        public int? ID {  get; set; }
        public string? DeptID {  get; set; }
        public string? Name {  get; set; }
        public int? UnitID {  get; set; }
        public int? ParentOrgUnitID {  get; set; }
        public bool? IsManagement {  get; set; }
        public int? ManagerID { get; set; }
        public int? DeputyID { get; set; }

    }
}
