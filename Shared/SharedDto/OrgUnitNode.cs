namespace ServicePortals.Application.Dtos.User.Requests
{
    public class OrgUnitNode
    {
        public int? OrgUnitId { get; set; }
        public string? OrgUnitName { get; set; }
        public int? ParentJobTitleId { get; set; }
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public List<Person> People { get; set; } = new();
        public List<OrgUnitNode> Children { get; set; } = [];
        public string? ParentName { get; set; }
    }

    public class Person
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
    }
}
