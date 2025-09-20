using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("refresh_tokens")]
    [Index(nameof(Token))]
    [Index(nameof(UserCode))]
    [Index(nameof(ExpiresAt))]
    public class RefreshToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string? Token { get; set; }

        [MaxLength(30)]
        public string? UserCode { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
