namespace ServicePortals.Application.Dtos.DelegatedTemp.Requests
{
    public class CreateDelegatedTempRequest
    {
        public int? MainOrgUnitId {  get; set; }
        public string? MainUserCode { get; set; }
        public string? TempUserCode { get; set; }
        public int? RequestTypeId { get; set; } = 1;
    }
}
