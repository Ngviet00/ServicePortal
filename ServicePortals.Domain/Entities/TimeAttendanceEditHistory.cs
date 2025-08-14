using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("time_attendance_edit_histories")]
    public class TimeAttendanceEditHistory
    {
        public int? Id { get; set; }                   
        public DateTimeOffset? Datetime { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public string? OldValue { get; set; }
        public string? CurrentValue { get; set; }
        public string? UserCodeUpdated { get; set; }
        public string? UpdatedBy { get; set; }
        public bool? IsSentToHR { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}