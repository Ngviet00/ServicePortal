using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("history_application_forms"), Index(nameof(ApplicationFormId), nameof(UserApproval))]
    public class HistoryApplicationForm
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? UserApproval { get; set; }
        public string? UserCodeApproval { get; set; }
        public string? ActionType { get; set; } //APPROVED, REJECT
        public string? Comment { get; set; } //ghi chú
        public DateTimeOffset? CreatedAt { get; set; }
        public User? User { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
    }
}
