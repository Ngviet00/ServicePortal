using System.Text.Json.Serialization;

namespace ServicePortal.Modules.Department.DTO.Requests
{
    public class GetAllDepartmentRequestDto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; } //search by name

        [JsonPropertyName("page")]
        public int Page { get; set; } = 1; //current page

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; } = 5; //each item in per page
    }
}
