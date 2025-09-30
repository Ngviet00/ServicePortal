namespace ServicePortals.Application.Dtos.InternalMemoHR.Requests
{
    public class CreateInternalMemoHrRequest
    {
        public int OrgPositionId { get; set; }
        public int DepartmentId { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? UserNameCreated { get; set; }
        public string? Title { get; set; }
        public string? TitleOther { get; set; }
        public string? Save { get; set; }
        public string? Note { get; set; }
        public List<string> Headers { get; set; } = [];
        public List<List<string>> Rows { get; set; } = new();
    }
}
