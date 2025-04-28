using System.Text.Json.Serialization;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Position.DTO;

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

        [JsonPropertyName("sex")]
        public byte? Sex { get; set; }

        [JsonPropertyName("role")]
        public Domain.Entities.Role? Role { get; set; }

        [JsonPropertyName("parent_department")]
        public DepartmentDTO? ParentDepartment { get; set; }

        [JsonPropertyName("children_department")]
        public DepartmentDTO? ChildrenDepartment { get; set; }

        [JsonPropertyName("position")]
        public PositionDTO? Position { get; set; }
    }
}