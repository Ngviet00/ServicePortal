using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("it_categories")]
    public class ITCategory
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
