namespace ServicePortals.Domain.Entities
{
    public class TimeAttendanceEditHistory
    {
        public int Id { get; set; }                   
        public int RecordId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string EditedBy { get; set; } = string.Empty;
        public DateTime EditedAt { get; set; }         
        public bool IsSentToHR { get; set; }            
    }
}