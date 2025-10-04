namespace ServicePortals.Shared.SharedDto.Requests
{
    public class ResolvedTaskRequest
    {
        public string? UserCodeApproval { get; set; }
        public string? UserNameApproval { get; set; }
        public int? OrgPositionId { get; set; }
        public long ApplicationFormId { get; set; }
        public Guid ITFormId { get; set; }
        public Guid PurchaseId { get; set; }
        public string? UrlFrontend { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
    }
}
