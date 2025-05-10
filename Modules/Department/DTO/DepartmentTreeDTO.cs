//using ServicePortal.Modules.Position.DTO;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Department.DTO
{
    public class DepartmentTreeDTO
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; } = null;

        //[JsonPropertyName("positions")]
        //public List<PositionDTO> Positions { get; set; } = new();

        [JsonPropertyName("childrens")]
        public List<DepartmentTreeDTO> Childrens { get; set; } = new();
    }
}
