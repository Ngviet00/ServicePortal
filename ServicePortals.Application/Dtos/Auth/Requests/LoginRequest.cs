using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Auth.Requests
{
    public class LoginRequest
    {
        [Required]
        public string? UserCode {  get; set; }

        [Required]
        public string? Password {  get; set; }
    }
}
