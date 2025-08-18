using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //thời gian nghỉ - cả ngày, buổi sáng, buổi chiều
    [Table("time_leaves")]
    public class TimeLeave
    {
        public int? Id { get; set; }
        public string? Name { get; set; } //tiếng việt
        public string? NameE{ get; set; } //tiếng anh
    }
}
