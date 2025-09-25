using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("application_forms")]
    [Index(nameof(Id))]
    [Index(nameof(OrgPositionId))]
    [Index(nameof(Code))]
    public class ApplicationForm
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(100)]
        public string? Code { get; set; } //mã đơn

        public int RequestTypeId { get; set; } //loại đơn
        public int RequestStatusId { get; set; } //trạng thái của đơn
        public int OrgPositionId { get; set; } //vị trí người tiếp theo duyệt đơn này

        [MaxLength(30)]
        public string? UserCodeCreatedForm { get; set; } //mã nhân viên người tạo đơn

        [MaxLength(50)]
        public string? UserNameCreatedForm { get; set; } //tên người tạo đơn

        public int? DepartmentId { get; set; } //cho việc filter, department id của người tạo đơn
        public string? Note { get; set; }
        public int Step { get; set; }
        public string? MetaData { get; set; } //type json

        public int? TypeOverTimeId { get; set; }
        public int? OrgUnitCompanyId { get; set; }
        public DateTimeOffset? DateRegister { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        //relationship
        public OrgUnit? OrgUnitCompany { get; set; }
        public OrgUnit? OrgUnit { get; set; }
        public TypeOverTime? TypeOverTime { get; set; }
        public RequestType? RequestType { get; set; } //foreign key RequestTypeId
        public RequestStatus? RequestStatus { get; set; } //foreign key RequestStatusId
        public ICollection<AssignedTask> AssignedTasks { get; set; } = []; //assigned task, đơn này sếp gán cho nhân viên nào,..
        public ICollection<ApplicationFormItem> ApplicationFormItems { get; set; } = []; //khi đăng ký nhiều người cùng 1 đơn
        public ICollection<HistoryApplicationForm> HistoryApplicationForms { get; set; } = []; //lịch sử của form, ai approved, ai tạo,...
    }
}
