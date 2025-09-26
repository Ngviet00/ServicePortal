using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.OverTime.Responses
{
    public class MyOverTimeResponse
    {
        public string? Code { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public string? Position { get; set; }
        public DateTimeOffset? DateRegister { get; set; }

        public string? TypeOverTimeId { get; set; }
        public string? TypeOverTimeName { get; set; }
        public string? TypeOverTimeNameE { get; set; }

        public string? FromHour { get; set; }
        public string? ToHour { get; set; }
        public string? NumberHour { get; set; }

        public string? Note { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }

        public string? RequestStatusId { get; set; }
        public string? RequestStatusName { get; set; }
        public string? RequestStatusNameE { get; set; }
    }
}
