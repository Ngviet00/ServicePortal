using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //loại phép
    [Table("type_leaves")]
    public class TypeLeave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(30)]
        public string? Name { get; set; } //tiếng việt

        [MaxLength(30)]
        public string? NameE { get; set; } //tiếng anh

        [MaxLength(30)]
        public string? Code { get; set; } //mã loại phép AL, MAT,...
    }
}
