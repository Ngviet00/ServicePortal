using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Applications.Modules.Role.DTO.Requests
{
    public class UpdateRoleDto
    {
        [Required, JsonPropertyName("id")]
        public int? Id { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
