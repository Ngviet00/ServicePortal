using System.Text.Json.Serialization;
using ServicePortal.Modules.Deparment.DTO;

namespace ServicePortal.Modules.User.DTO
{
    public class UserDTO
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("role_id")]
        public int? RoleId { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("date_join_company")]
        public DateTime? DateJoinCompany { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("level")]
        public string? Level { get; set; }

        [JsonPropertyName("level_parent")]
        public string? LevelParent { get; set; }

        [JsonPropertyName("sex")]
        public byte? Sex { get; set; }

        [JsonPropertyName("role")]
        public Domain.Entities.Role? Role { get; set; }

        [JsonPropertyName("department")]
        public DepartmentDTO? Department { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}