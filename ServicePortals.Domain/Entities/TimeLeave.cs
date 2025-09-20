using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //thời gian nghỉ - cả ngày, buổi sáng, buổi chiều
    [Table("time_leaves")]
    public class TimeLeave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(30)]
        public string? Name { get; set; } //tiếng việt

        [MaxLength(30)]
        public string? NameE{ get; set; } //tiếng anh
    }
}
