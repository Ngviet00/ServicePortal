using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("sap_types")]
    public class SAPType
    {
        public int? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
