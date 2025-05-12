using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Auth.DTO.Requests
{
    public class LoginRequestDto
    {
        [Required, JsonPropertyName("user_code")]
        public string? UserCode {  get; set; }

        [Required, JsonPropertyName("password")]
        public string? Password {  get; set; }
    }
}
