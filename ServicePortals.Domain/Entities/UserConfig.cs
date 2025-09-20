using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("user_configs"), Index(nameof(UserCode))]
    public class UserConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(30)]
        public string? UserCode { get; set; }

        [MaxLength(70)]
        public string? Key { get; set; }

        [MaxLength(70)]
        public string? Value { get; set; }
        public User? User { get; set; }
    }
}
