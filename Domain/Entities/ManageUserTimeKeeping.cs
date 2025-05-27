using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("manage_user_time_keeping"), Index(nameof(UserCodeManage), nameof(UserCode))]
    public class ManageUserTimeKeeping
    {
        public Guid? Id { get; set; }
        public string? UserCodeManage { get; set; }
        public string? UserCode {  get; set; }
    }
}
