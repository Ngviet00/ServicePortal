using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.TimeKeeping.Responses
{
    public class UserDailyRecord
    {
        public string? thu { get; set; }
        public string? bcNgay { get; set; }
        public DateTime? vao { get; set; }
        public DateTime? ra { get; set; }
        public string? result { get; set; }
        public string? bcGhiChu { get; set; }
    }

    public class GroupedUserTimeKeeping
    {
        public string? NVMaNV { get; set; }
        public string? NVHoTen { get; set; }
        public string? BPTen { get; set; }
        public List<UserDailyRecord> DataTimeKeeping { get; set; } = [];
    }
}
