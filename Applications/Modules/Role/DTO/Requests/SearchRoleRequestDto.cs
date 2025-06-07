using System.Text.Json.Serialization;

namespace ServicePortal.Applications.Modules.Role.DTO.Requests
{
    public class SearchRoleRequestDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; } //search by condition

        [JsonPropertyName("page")]
        public int Page { get; set; } = 1; //current page

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 20; //each item in per page
    }
}
