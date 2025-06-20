using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("user_manage_attendances")]
    public class UserManageAttendance
    {
        public int? Id { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
