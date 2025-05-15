using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Auth.DTO.Requests
{
    public class CreateUserDto
    {
        [Required, JsonPropertyName("usercode")]
        public string? UserCode { get; set; }

        [Required, JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}