using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("application_forms")]
    [Index(nameof(RequestStatusId))]
    [Index(nameof(OrgPositionId))]
    [Index(nameof(UserCodeRequestor))]
    public class ApplicationForm
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }

        public string? UserCodeRequestor { get; set; }
        public string? UserNameRequestor { get; set; }
        public string? UserCodeCreated { get; set; }
        public string? UserNameCreated { get; set; }

        public int? RequestTypeId { get; set; }
        public int? RequestStatusId { get; set; }
        public int? OrgPositionId { get; set; }
        public int? DepartmentId { get; set; }

        public int? Step { get; set; }
        public string? MetaData { get; set; } //json

        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public RequestType? RequestType { get; set; }
        public RequestStatus? RequestStatus { get; set; }
        public LeaveRequest? Leave { get; set; }
        public MemoNotification? MemoNotification { get; set; }
        public OrgPosition? OrgPosition { get; set; }
        public OrgUnit? OrgUnit { get; set; }
        public ITForm? ITForm { get; set; }
        public Purchase? Purchase { get; set; }
        public ICollection<AssignedTask> AssignedTasks { get; set; } = [];
        public ICollection<HistoryApplicationForm> HistoryApplicationForms { get; set; } = [];
    }
}
