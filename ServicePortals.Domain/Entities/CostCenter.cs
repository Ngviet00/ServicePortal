using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("cost_centers")]
    public class CostCenter
    {
        public int Id { get; set; }
        public string? Code { get;set; }
        public string? Description { get; set; }
    }
}
