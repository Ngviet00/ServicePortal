using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("request_statuses")]
    public class RequestStatus
    {
        public int? Id { get; set; }

        [StringLength(30)]
        public string? Name { get; set; }
    }
}
