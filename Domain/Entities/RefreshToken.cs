using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("token")]
        public string? Token { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [Column("is_revoked")]
        public bool? IsRevoked { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
    }
}
