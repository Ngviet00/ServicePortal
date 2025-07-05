using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("type_leaves")]
    public class TypeLeave
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }

        public static explicit operator int(TypeLeave v)
        {
            throw new NotImplementedException();
        }
    }
}
