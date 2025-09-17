using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.ITForm.Responses
{
    public class GetListITFormResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationFormItemId { get; set; }
        public int DepartmentId { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
        public int PriorityId { get; set; }
        public string NoteManagerIT { get; set; }
        public DateTimeOffset RequestDate { get; set; }
        public DateTimeOffset RequiredCompletionDate { get; set; }
        public DateTimeOffset? TargetCompletionDate { get; set; }
        public DateTimeOffset? ActualCompletionDate { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string Code { get; set; }
        public string DepartmentName { get; set; }
        public int RequestStatusId { get; set; }
    }
}
