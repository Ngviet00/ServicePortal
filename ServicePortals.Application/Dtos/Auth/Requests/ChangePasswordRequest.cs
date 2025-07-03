using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Auth.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string? NewPassword { get; set; }

        [Required, Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
        public string? Email { get; set; }
    }
}
