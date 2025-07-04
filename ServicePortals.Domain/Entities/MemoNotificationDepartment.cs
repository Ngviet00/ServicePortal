using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("memo_notification_departments"), Index(nameof(MemoNotificationId), nameof(DepartmentId))]
    public class MemoNotificationDepartment
    {
        public Guid? Id { get; set; }
        public Guid? MemoNotificationId { get; set; }
        public int? DepartmentId { get; set; }
        public MemoNotification? MemoNotifications { get; set; }
    }
}
