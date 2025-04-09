using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Application.DTOs.Auth.Requests
{
    public class LoginRequest
    {
        [Required]
        [JsonPropertyName("user_code")]
        public string? UserCode {  get; set; }

        [Required]
        [JsonPropertyName("password")]
        public string? Password {  get; set; }
    }
}
