namespace ServicePortals.Application.Dtos.DelegatedTemp.Responses
{
    public class GetAllDelegatedTempResponse
    {
        public int? MainOrgUnitId {  get; set; }
        public string? OrgUnitName { get; set; }
        public string? MainUser { get; set; }
        public string? MainUserCode { get; set; }
        public string? TempUser { get; set; }
        public string? TempUserCode { get; set; }
    }
}
