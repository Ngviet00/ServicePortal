using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("application_forms"), Index(nameof(RequesterUserCode))]
    [Index(nameof(RequestStatusId), nameof(CurrentOrgUnitId))]
    public class ApplicationForm
    {
        public Guid? Id { get; set; }
        public string? RequesterUserCode { get; set; }
        public int? RequestTypeId { get; set; }
        public int? RequestStatusId { get; set; } //PENDING, IN-PROCESS, COMPLETE
        public int? CurrentOrgUnitId { get; set; } //current org unit id will approval
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public User? User { get; set; }
        public RequestType? RequestType { get; set; }
        public RequestStatus? RequestStatus { get; set; }
    }
}
