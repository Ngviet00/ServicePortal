using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("it_categories")]
    public class ITCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(30)]
        public string? Name { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
    }
}
