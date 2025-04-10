using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Deparment.DTO
{
    public class DeparmentDTO
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; } = null;
    }
}
