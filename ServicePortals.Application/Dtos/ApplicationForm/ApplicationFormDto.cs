using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.ApplicationForm
{
    public class ApplicationFormDto
    {
        public Guid? Id { get; set; }
        public string? RequesterUserCode { get; set; }
        public int? RequestTypeId { get; set; }
        public int? RequestStatusId { get; set; } //PENDING, IN-PROCESS, COMPLETE
        public int? CurrentOrgUnitId { get; set; } //current org unit id will approval
        public DateTimeOffset? CreatedAt { get; set; }
        public RequestType? RequestType { get; set; }
        public RequestStatus? RequestStatus { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
