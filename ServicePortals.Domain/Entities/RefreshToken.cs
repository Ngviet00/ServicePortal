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
        public Guid Id { get; set; }
        public string? Token { get; set; }
        public string? UserCode { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool? IsRevoked { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
