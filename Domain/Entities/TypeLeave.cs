using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServicePortal.Domain.Entities
{
    [Table("type_leaves")]
    public class TypeLeave
    {
        [Column("id")]
        public int Id { get; set; }

        [MaxLength(50), Column("name")]
        public string? Name { get; set; }

        [MaxLength(200), Column("note")]
        public string? Note { get; set; }

        [MaxLength(100), Column("modified_by")]
        public string? ModifiedBy { get; set; }

        [MaxLength(200), Column("modified_at")]
        public DateTime? ModifiedAt { get; set; }
    }
}
