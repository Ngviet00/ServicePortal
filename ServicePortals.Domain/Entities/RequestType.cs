using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("request_types")]
    public class RequestType
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? NameE { get; set; }
    }
}
