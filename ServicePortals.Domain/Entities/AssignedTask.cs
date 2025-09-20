using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("assigned_tasks")]
    public class AssignedTask
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long ApplicationFormId { get; set; }

        [MaxLength(30)]
        public string? UserCode { get; set; }
    }
}
