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
        public Guid? Id { get; set; }
        public string? UserCodeRequestor { get; set; } //mã nhân viên người yêu cầu
        public string? UserNameRequestor { get; set; } //tên nhân viên người yêu cầu
        public int? RequestTypeId { get; set; } //loại yêu cầu, mua bán, form it, sap
        public int? RequestStatusId { get; set; } //trạng thái của đơn như pending, complete,...
        public int? OrgPositionId { get; set; } //vị trí người duyệt
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public ICollection<HistoryApplicationForm> HistoryApplicationForms { get; set; } = [];
        public RequestType? RequestType { get; set; }
        public RequestStatus? RequestStatus { get; set; }
        public LeaveRequest? Leave { get; set; }
        public MemoNotification? MemoNotification { get; set; }
        public OrgPosition? OrgPosition { get; set; }
        public ITForm? ITForm { get; set; }
        public Purchase? Purchase { get; set; }
        public ICollection<AssignedTask> AssignedTasks { get; set; } = [];
    }
}
