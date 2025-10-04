using Microsoft.AspNetCore.Http;

namespace ServicePortals.Application.Dtos.ITForm.Requests
{
    public class UpdateITFormRequest
    {
        public string? Email { get; set; }
        public string? Position { get; set; }
        public string? Reason { get; set; }
        public int? PriorityId { get; set; }
        public DateTimeOffset? RequestDate { get; set; }
        public DateTimeOffset? RequiredCompletionDate { get; set; }
        public IFormFile[] Files { get; set; } = [];
        public List<long> IdDeleteFile { get; set; } = [];
    }
}
