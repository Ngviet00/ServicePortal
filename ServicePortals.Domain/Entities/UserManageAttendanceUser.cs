using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("user_manage_attendance_users"), Index(nameof(UserCodeManage), nameof(UserCode))]
    public class UserManageAttendanceUser
    {
        public Guid? Id { get; set; }
        public string? UserCodeManage { get; set; }
        public string? UserCode { get; set; }
    }
}