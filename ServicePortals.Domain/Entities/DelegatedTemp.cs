using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("delegated_temp")]
    public class DelegatedTemp
    {
        public int? Id { get; set; }
        public int? MainOrgUnitId { get; set; } //vị trí chính
        public string? TempUserCode { get; set; } //người phụ
        public int? RequestTypeId { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool? IsPermanent { get; set; }
        public bool? IsActive { get; set; }
    }
}
