using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("memo_notification_departments"), Index(nameof(MemoNotificationId), nameof(DepartmentId))]
    public class MemoNotificationDepartment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long MemoNotificationId { get; set; }
        public int DepartmentId { get; set; }
        public MemoNotification? MemoNotifications { get; set; }
        public OrgUnit? OrgUnit { get; set; }
    }
}
