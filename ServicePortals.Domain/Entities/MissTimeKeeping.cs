using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("miss_timekeepings"), Index(nameof(ApplicationFormItemId))]
    public class MissTimeKeeping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ApplicationFormItemId { get; set; }
        
        [MaxLength(30)]
        public string? UserCode { get; set; }
        
        [MaxLength(50)]
        public string? UserName { get; set; }
        
        public DateTimeOffset? DateRegister { get; set; }
        
        [MaxLength(20)]
        public string? Shift { get; set; }
        
        [MaxLength(20)]
        public string? AdditionalIn { get; set; } //giờ bù dữ liệu vào
        
        [MaxLength(20)]
        public string? AdditionalOut { get; set; } //giờ bù dữ liệu ra
        
        [MaxLength(20)]
        public string? FacialRecognitionIn { get; set; } //nhận diện khuân mặt vào
        
        [MaxLength(20)]
        public string? FacialRecognitionOut { get; set; } //nhận diện khuân mặt ra

        [MaxLength(20)]
        public string? GateIn { get; set; } //giờ cổng vào

        [MaxLength(20)]
        public string? GateOut { get; set; } //giờ cổng ra

        [MaxLength(255)]
        public string? Reason { get; set; }
        public string? NoteOfHR { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        
        public ApplicationFormItem? ApplicationFormItem { get; set; }
    }
}
