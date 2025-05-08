using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Role.Requests
{
    public class CreateRoleRequest
    {
        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }


        [JsonPropertyName("code")]
        public string? Code { get; set; }
    }
}
