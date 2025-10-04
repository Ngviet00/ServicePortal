namespace ServicePortals.Application.Dtos.ITForm.Responses
{
    public class GetListITFormResponse
    {
        public long Id { get; set; }
        public long ApplicationFormItemId { get; set; }
        public int DepartmentId { get; set; }
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? Reason { get; set; }
        public int PriorityId { get; set; }
        public string? NoteManagerIT { get; set; }
        public DateTimeOffset? RequestDate { get; set; }
        public DateTimeOffset? RequiredCompletionDate { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? Code { get; set; }
        public string? DepartmentName { get; set; }
        public string? CreatedBy { get; set; }
        public int? RequestStatusId { get; set; }
    }
}
