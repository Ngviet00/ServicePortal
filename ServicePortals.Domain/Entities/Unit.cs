using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("units")]
    public class Unit
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
    }
}
