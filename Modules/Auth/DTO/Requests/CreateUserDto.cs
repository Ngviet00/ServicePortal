using System.ComponentModel.DataAnnotations;

namespace ServicePortal.Modules.Auth.DTO.Requests
{
    public class CreateUserDto
    {
        [Required]
        public string? UserCode { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}