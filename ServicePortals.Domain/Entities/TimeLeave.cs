using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("time_leaves")]
    public class TimeLeave
    {
        public int? Id { get; set; }

        [StringLength(30)]
        public string? Description { get; set; }
    }
}
