namespace ServicePortals.Application.Dtos.MemoNotification.Responses
{
    public class GetAllMemoNotifyResponse
    {
        public string? Code { get; set; }
        public long Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Departments { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool Status { get; set; }
        public bool ApplyAllDepartment { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public int RequestStatusId { get; set; }
        public string? UserNameCreatedForm { get; set; }
    }
}
