using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.TimeKeeping.Responses
{
    public class GetUserTimeKeepingResponse
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public string? BPTen { get; set; }
        public string? Thu { get; set; }
        public string? BCNgay { get; set; }
        public string? BCGhiChu { get; set; }
        public DateTime? BCTGDen { get; set; }
        public DateTime? BCTGVe { get; set; }
        public string? Results { get; set; }
    }
}
