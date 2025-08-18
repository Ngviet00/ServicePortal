using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("delegations")]
    public class Delegation
    {
        public int? Id { get; set; }
        public int? PositionId { get; set; }
        public string? FromUserCode { get; set; }
        public string? ToUserCode { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool? IsPermanent { get; set; }
        public bool? IsActive { get; set; }
    }
}
