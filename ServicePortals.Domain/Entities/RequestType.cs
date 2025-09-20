using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //loại đơn - nghỉ phép, mua bán, it, sap,...
    [Table("request_types")]
    public class RequestType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; } //tiếng việt

        [MaxLength(50)]
        public string? NameE { get; set; } //tiếng anh
    }
}
