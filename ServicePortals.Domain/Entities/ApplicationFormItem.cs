using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("application_form_items")]
    [Index(nameof(Id))]
    [Index(nameof(ApplicationFormId))]
    public class ApplicationFormItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ApplicationFormId { get; set; }

        [MaxLength(30)]
        public string? UserCode { get; set; }

        [MaxLength(50)]
        public string? UserName { get; set; }

        public bool Status { get; set; } = true;
        public string? Note { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? RejectedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        //relationship
        public ApplicationForm? ApplicationForm { get; set; } //ApplicationFormId
        public List<LeaveRequest> LeaveRequests { get; set; } = [];
        public List<MemoNotification> MemoNotifications { get; set; } = [];
        public List<ITForm> ITForms { get; set; } = [];
        public List<Purchase> Purchases { get; set; } = [];
    }
}
