using System.Text.Json.Serialization;

namespace ServicePortals.Application.Dtos.User.Requests
{
    public class UpdateUserRoleRequest
    {
        [JsonPropertyName("user_code")]
        public string? UserCode { get; set; }

        [JsonPropertyName("role_ids")]
        public List<int>? RoleIds { get; set; } = [];
    }
}
