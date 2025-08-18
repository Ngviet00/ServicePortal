using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Priority.Requests
{
    public class UpdatePriorityRequest
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? NameE { get; set; }
    }
}
