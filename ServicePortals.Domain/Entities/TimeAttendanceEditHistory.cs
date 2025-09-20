using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("time_attendance_edit_histories")]
    public class TimeAttendanceEditHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }                   
        public DateTimeOffset? Datetime { get; set; }

        [MaxLength(30)]
        public string? UserCode { get; set; }

        [MaxLength(30)]
        public string? UserName { get; set; }

        [MaxLength(30)]
        public string? OldValue { get; set; }

        [MaxLength(30)]
        public string? CurrentValue { get; set; }

        [MaxLength(30)]
        public string? UserCodeUpdated { get; set; }

        [MaxLength(30)]
        public string? UpdatedBy { get; set; }

        public bool IsSentToHR { get; set; } = false;
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}