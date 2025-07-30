namespace ServicePortals.Application.Dtos.User.Requests
{
    public class OrgUnitNode
    {
        public int? OrgUnitId { get; set; }
        public string? OrgUnitName { get; set; }
        public int? ParentJobTitleId { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public List<OrgUnitNode> Children { get; set; } = [];
        public string? ParentName { get; set; }
    }

    public class Person
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
    }

    public class OrgUnitNode_1
    {
        public int? Id { get; set; }
        public int? DeptId { get; set; }
        public string? Name { get; set; }
        public int? UnitId { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int? ParentJobTitleId { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public List<OrgUnitNode_1> Children { get; set; } = [];
    }
}