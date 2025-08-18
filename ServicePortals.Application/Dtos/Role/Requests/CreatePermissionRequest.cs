using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Role.Requests
{
    public class CreatePermissionRequest
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Group { get; set; }
    }
}
