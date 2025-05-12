using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Auth.DTO.Requests
{
    public class CreateUserDto
    {
        [Required, JsonPropertyName("code")]
        public string? Code { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [Required, JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("date_join_company")]
        public DateTime? DateJoinCompany { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("sex")]
        public byte? Sex { get; set; }

        [Required, JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        [Required, JsonPropertyName("level")]
        public string? Level { get; set; }

        [Required, JsonPropertyName("level_parent")]
        public string? LevelParent { get; set; }

        [Required, JsonPropertyName("role_id")]
        public int? RoleId { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }
    }
}