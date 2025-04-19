using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Position.DTO
{
    public class PositionDTO
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("position_level")]
        public int? PositionLevel { get; set; }
    }
}
