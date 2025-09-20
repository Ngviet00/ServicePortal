using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("system_configs")]
    public class SystemConfig
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? ConfigKey { get; set; }
        public string? ConfigValue { get; set; }
        public string? ValueType { get; set; }
        public string? DefaultValue { get; set; }
        public string? MinValue { get; set; }
        public string? MaxValue { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
