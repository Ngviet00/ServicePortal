using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Role.Requests
{
    public class UpdateRoleRequest
    {
        [Required, JsonPropertyName("id")]
        public int? Id { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
