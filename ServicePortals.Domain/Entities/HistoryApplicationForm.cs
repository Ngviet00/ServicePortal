using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("history_application_forms"), Index(nameof(ApplicationFormId))]
    public class HistoryApplicationForm
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? Note { get; set; }
        public string? Action { get; set; } //Created, Submitted, Approved, Rejected, Cancelled
        public string? ActionBy { get; set; }
        public DateTimeOffset? ActionAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }


        //relationship
        public ApplicationForm? ApplicationForm { get; set; } //ApplicationFormId
    }
}
