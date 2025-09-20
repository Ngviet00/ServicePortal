namespace ServicePortals.Application.Dtos.OrgUnit.Requests
{
    public class SaveOrUpdateOrgUnitRequest
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int UnitId { get; set; }
    }
}
