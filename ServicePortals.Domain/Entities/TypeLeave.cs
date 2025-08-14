using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //loại phép
    [Table("type_leaves")]
    public class TypeLeave
    {
        public int Id { get; set; }
        public string? Name { get; set; } //tiếng việt
        public string? NameE { get; set; } //tiếng anh
        public string? Code { get; set; } //mã loại phép AL, MAT,...
    }
}
