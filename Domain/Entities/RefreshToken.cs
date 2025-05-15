using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("refresh_tokens"), Index(nameof(Token), nameof(UserCode), nameof(ExpiresAt), nameof(IsRevoked))]
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string? Token { get; set; }
        public string? UserCode { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public bool? IsRevoked { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
