using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("priorities")]
    public class Priority
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? NameE {  get; set; }
    }
}
