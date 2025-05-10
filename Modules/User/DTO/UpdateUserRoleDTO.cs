using System.Text.Json.Serialization;

namespace ServicePortal.Modules.User.DTO
{
    public class UpdateUserRoleDTO
    {
        [JsonPropertyName("user_code")]
        public string? UserCode { get; set; }

        [JsonPropertyName("role_ids")]
        public List<int>? RoleIds { get; set; } = [];
    }
}
