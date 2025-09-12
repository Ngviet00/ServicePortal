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

        //relationship
        public ApplicationForm? ApplicationForm { get; set; } //ApplicationFormId
        public List<LeaveRequest> LeaveRequests { get; set; } = [];
    }
}
