using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("application_forms")]
    [Index(nameof(RequestStatusId))]
    [Index(nameof(OrgPositionId))]
    [Index(nameof(UserCodeCreatedBy))]
    public class ApplicationForm
    {
        public Guid Id { get; set; }
        public string? Code { get; set; } //mã đơn
        public int? RequestTypeId { get; set; } //loại đơn
        public int? RequestStatusId { get; set; } //trạng thái của đơn
        public int? OrgPositionId { get; set; } //vị trí người tiếp theo duyệt đơn này
        public string? UserCodeCreatedBy { get; set; } //mã nhân viên người tạo đơn
        public string? CreatedBy { get; set; } //tên người tạo đơn
        public string? Note { get; set; }
        public int? Step { get; set; }
        public string? MetaData { get; set; } //type json
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }


        //relationship
        public RequestType? RequestType { get; set; } //foreign key RequestTypeId
        public RequestStatus? RequestStatus { get; set; } //foreign key RequestStatusId
        public ICollection<AssignedTask> AssignedTasks { get; set; } = []; //assigned task, đơn này sếp gán cho nhân viên nào,..
        public ICollection<ApplicationFormItem> ApplicationFormItems { get; set; } = []; //khi đăng ký nhiều người cùng 1 đơn
        public ICollection<HistoryApplicationForm> HistoryApplicationForms { get; set; } = []; //lịch sử của form, ai approved, ai tạo,...

        //public LeaveRequest? Leave { get; set; }
        //public MemoNotification? MemoNotification { get; set; }
        //public OrgPosition? OrgPosition { get; set; }
        //public OrgUnit? OrgUnit { get; set; }
        //public ITForm? ITForm { get; set; }
        //public Purchase? Purchase { get; set; }
    }
}
