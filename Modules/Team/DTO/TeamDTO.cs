using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ServicePortal.Modules.Deparment.DTO;

namespace ServicePortal.Modules.Team.DTO
{
    public class TeamDTO
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        public DepartmentDTO? departmentDto { get; set; }
    }
}
