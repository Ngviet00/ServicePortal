namespace ServicePortals.Shared.SharedDto.Requests
{
    public class AssignedTaskRequest
    {
        public string? UserCodeApproval { get; set; }
        public string? UserNameApproval { get; set; }
        public string? NoteManager { get; set; }
        public int? OrgPositionId { get; set; }
        public Guid? ITFormId { get; set; }
        public Guid? PurchaseId { get; set; }
        public string? UrlFrontend { get; set; }
        public string? Note { get; set; }
        public List<UserAssignedTaskRequest> UserAssignedTasks { get; set; } = [];
    }

    public class UserAssignedTaskRequest
    {
        public string? UserCode { get; set; }
        public string? Email { get; set; }
    }
}
