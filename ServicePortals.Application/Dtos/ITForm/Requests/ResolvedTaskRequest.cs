using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.ITForm.Requests
{
    public class ResolvedTaskRequest
    {
        public string? UserCodeApproval { get; set; }
        public string? UserNameApproval { get; set; }
        public int? OrgPositionId { get; set; }
        public Guid? ITFormId { get; set; }
        public string? UrlFrontend { get; set; }
    }
}
