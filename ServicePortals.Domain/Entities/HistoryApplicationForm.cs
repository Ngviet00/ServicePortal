using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("history_application_forms"), Index(nameof(ApplicationFormId))]
    public class HistoryApplicationForm
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? UserNameApproval { get; set; }
        public string? UserCodeApproval { get; set; }
        public string? Action { get; set; } //APPROVED or REJECT
        public string? Note { get; set; } //ghi chú
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
    }
}
