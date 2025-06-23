using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("user_configs"), Index(nameof(UserCode))]
    public class UserConfig
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? ConfigKey { get; set; }
        public string? ConfigValue { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}
