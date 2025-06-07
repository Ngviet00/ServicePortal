using System.ComponentModel.DataAnnotations;

namespace ServicePortal.Applications.Modules.Auth.DTO.Requests
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string? NewPassword { get; set; }

        [Required, Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
