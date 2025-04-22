using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("positions"), Index(nameof(Id))]
    public class Position
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string? Name { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("department_id")]
        public int? DepartmentId { get; set; }

        [Column("level")]
        public int? Level{ get; set; }

        [Column("is_global")]
        public bool? IsGlobal { get; set; } = false;
    }
}
