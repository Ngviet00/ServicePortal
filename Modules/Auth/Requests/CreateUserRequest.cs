using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Auth.Requests
{
    public class CreateUserRequest
    {
        [Required, JsonPropertyName("code")]
        public string? Code { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [Required, JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [Required, JsonPropertyName("role_id")]
        public int? RoleId { get; set; }

        [JsonPropertyName("date_join_company")]
        public DateTime? DateJoinCompany { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }
        
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("sex")]
        public byte? Sex { get; set; }

        [Required, JsonPropertyName("parent_department_id")]
        public int? ParentDepartmentId { get; set; }

        [JsonPropertyName("child_deparment_id")]
        public int? ChildDepartmentId { get; set; }

        [Required, JsonPropertyName("position_id")]
        public int? PositionId { get; set; }

        [JsonPropertyName("management_position_id")]
        public int? ManagementPositionId { get; set; }
    }
}