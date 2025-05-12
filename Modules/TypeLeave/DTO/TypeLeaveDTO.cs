using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServicePortal.Modules.TypeLeave.DTO
{
    public class TypeLeaveDto
    {
        [Required, JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("modified_by")]
        public string? ModifiedBy { get; set; }
    }
}
