using System.Text.Json.Serialization;

namespace ServicePortal.Applications.Modules.User.DTO.Requests
{
    public class UpdateUserRoleDto
    {
        [JsonPropertyName("user_code")]
        public string? UserCode { get; set; }

        [JsonPropertyName("role_ids")]
        public List<int>? RoleIds { get; set; } = [];
    }
}
