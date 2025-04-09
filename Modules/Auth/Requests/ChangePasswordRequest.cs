using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Application.DTOs.Auth.Requests
{
    public class ChangePasswordRequest
    {
        [Required, JsonPropertyName("new_password")]
        public string? NewPassword { get; set; }

        [JsonPropertyName("confirm_password"), Required, Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
    }
}
