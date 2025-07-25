namespace ServicePortals.Application.Dtos.MemoNotification.Requests
{
    public class HistoryWaitApprovalMemoNotifyRequest
    {
        public string? CurrentUserCode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
