using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("history_application_forms"), Index(nameof(ApplicationFormId))]
    public class HistoryApplicationForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ApplicationFormId { get; set; }
        public string? Note { get; set; }

        [MaxLength(30)]
        public string? Action { get; set; } //Created, Submitted, Approved, Rejected, Cancelled

        [MaxLength(30)]
        public string? UserCodeAction { get; set; }

        [MaxLength(30)]
        public string? UserNameAction { get; set; }
        public DateTimeOffset? ActionAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        //relationship
        public ApplicationForm? ApplicationForm { get; set; } //ApplicationFormId
    }
}
