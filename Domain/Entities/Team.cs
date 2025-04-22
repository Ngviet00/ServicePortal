using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ServicePortal.Domain.Entities
{
    [Table("teams")]
    public class Team
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name"), MaxLength(50)]
        public string? Name { get; set; }

        [Column("department_id")]
        public int? DepartmentId { get; set; }
    }
}
