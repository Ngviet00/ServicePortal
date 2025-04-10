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

        [Required, JsonPropertyName("position_id")]
        public int? PositionId { get; set; }

        [Required, JsonPropertyName("deparment_id")]
        public int? DeparmentId { get; set; }

        [Required, JsonPropertyName("position_deparment_level")]
        public int? PositionDeparmentLevel { get; set; }

        [JsonPropertyName("date_join_company")]
        public DateTime? DateJoinCompany { get; set; }
    }
}