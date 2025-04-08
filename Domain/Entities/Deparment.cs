using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domains.Models
{
    [Table("deparments")]
    public class Deparment
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name"), MaxLength(50)]
        public string? Name { get; set; }

        [Column("note"), MaxLength(255)]
        public string? Note { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
