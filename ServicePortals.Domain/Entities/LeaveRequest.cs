using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("leave_requests"), Index(nameof(Id), nameof(UserCodeRequestor))]
    public class LeaveRequest
    {
        public Guid Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? Code { get; set; } //mã đơn
        public string? UserCodeRequestor { get; set; } //mã nhân viên người xin nghỉ
        public string? UserNameRequestor { get; set; } //tên nhân viên người xin nghỉ
        public int? DepartmentId { get; set; } //phòng ban
        public string? Position { get; set; } = "Staff"; //chức vụ, để mặc định là Staff
        public DateTimeOffset? FromDate { get; set; } //Ngày bắt đầu nghỉ
        public DateTimeOffset? ToDate { get; set; } //ngày kết thúc nghỉ
        public int? TypeLeaveId { get; set; } //loại phép
        public int? TimeLeaveId { get; set; } //thời gian nghỉ
        public string? Reason { get; set; } //lý do
        public byte? HaveSalary { get; set; }
        public string? UserCodeCreated { get; set; } //mã nhân viên người tạo
        public string? CreatedBy { get; set; } // tên nhân viên người tạo
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdateAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public User? User { get; set; }
        public ApplicationForm? ApplicationForm { get; set; }
        public TimeLeave? TimeLeave { get; set; }
        public TypeLeave? TypeLeave{ get; set; }
        public OrgUnit? OrgUnit { get; set; } //khóa ngoại departmentId liên kết vs Id trong bảng org unit để lấy thông tin phòng ban
    }
}
