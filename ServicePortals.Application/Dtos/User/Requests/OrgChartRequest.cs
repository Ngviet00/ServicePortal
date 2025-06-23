namespace ServicePortals.Application.Dtos.User.Requests
{
    public class OrgChartRequest
    {
        public int? PositionId { get; set; }
        public List<Person>? People { get; set; } = [];
        public List<OrgChartRequest>? Children { get; set; } = [];
    }

    public class Person
    {
        public string? Usercode { get; set; }
        public int? PositionId { get; set; }
    }
}
