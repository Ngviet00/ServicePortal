using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("over_times"), Index(nameof(ApplicationFormItemId))]
    public class OverTime
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ApplicationFormItemId { get; set; }
        [MaxLength(30)]
        public string? UserCode { get; set;}
        [MaxLength(50)]
        public string? UserName { get; set;}
        [MaxLength(50)]
        public string? Position { get; set;}
        public DateTimeOffset? FromHour { get; set; }
        public DateTimeOffset? ToHour { get; set; }
        public int NumberHour { get; set; }
        public string? Note { get; set; }
        public int TypeOverTimeId { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        public TypeOverTime? TypeOverTime { get; set; }
        public ApplicationFormItem? ApplicationFormItem { get; set; }
    }
}
