using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Priority.Requests
{
    public class CreatePriorityRequest
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? NameE { get; set; }
    }
}
