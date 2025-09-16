using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("application_form_items"), Index(nameof(ApplicationFormId))]
    public class ApplicationFormItem
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public bool Status { get; set; } = true; //false nghĩa là bị reject form đơn riêng lẻ
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
