using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //loại đơn - nghỉ phép, mua bán, it, sap,...
    [Table("request_types")]
    public class RequestType
    {
        public int? Id { get; set; }
        public string? Name { get; set; } //tiếng việt
        public string? NameE { get; set; } //tiếng anh
    }
}
