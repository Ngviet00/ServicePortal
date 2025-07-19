using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.DelegatedTemp
{
    public class DelegatedTempDto
    {
        public int? Id { get; set; }
        public int? MainOrgUnitId { get; set; } //vị trí chính
        public string? TempUserCode { get; set; } //người phụ
        public int? RequestTypeId { get; set; } = 1;
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public bool? IsPermanent { get; set; } = true;
        public bool? IsActive { get; set; } = true;
    }
}
