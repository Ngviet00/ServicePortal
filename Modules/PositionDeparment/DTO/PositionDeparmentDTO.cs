using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.PositionDeparment.DTO
{
    public class PositionDeparmentDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Required, JsonPropertyName("deparment_id")]
        public int? DeparmentId {  get; set; }

        [Required, JsonPropertyName("position_id")]
        public int? PositionId { get; set; }

        [Required, JsonPropertyName("position_deparment_level")]
        public int? PositionDeparmentLevel { get; set; }

        [JsonPropertyName("custom_title")]
        public string? CustomTitle { get; set; }
    }
}
