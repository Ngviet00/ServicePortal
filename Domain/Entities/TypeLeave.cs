using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("type_leaves")]
    public class TypeLeave
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; } = DateTimeOffset.Now;
    }
}
