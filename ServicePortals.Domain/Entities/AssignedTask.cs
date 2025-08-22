using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("assigned_tasks")]
    public class AssignedTask
    {
        public int Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? UserCode { get; set; }
    }
}
