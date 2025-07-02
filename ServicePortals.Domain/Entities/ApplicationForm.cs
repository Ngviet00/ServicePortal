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
        public DateTimeOffset? CreatedAt { get; set; }
        public User? User { get; set; }
        public RequestType? RequestType { get; set; }
        public RequestStatus? RequestStatus { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public ICollection<HistoryApplicationForm> HistoryApplicationForms { get; set; } = [];

    }
}
