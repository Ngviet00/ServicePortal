using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.TypeLeave.DTO
{
    public class TypeLeaveDTO
    {
        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("modified_by")]
        public string? ModifiedBy { get; set; }
    }
}
