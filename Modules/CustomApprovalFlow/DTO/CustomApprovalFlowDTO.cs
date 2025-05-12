using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace ServicePortal.Modules.CustomApprovalFlow.DTO
{
    public class CustomApprovalFlowDto
    {
        [JsonPropertyName("id"), FromQuery(Name = "id")]
        public int? Id { get; set; }

        [JsonPropertyName("department_id"), FromQuery(Name ="department_id")]
        public int? DepartmentId { get; set; }

        [JsonPropertyName("type_custom_approval"), FromQuery(Name = "type_custom_approval")]
        public string? TypeCustomApproval { get; set; }

        [JsonPropertyName("from"), FromQuery(Name = "from")]
        public string? From { get; set; }

        [JsonPropertyName("to"), FromQuery(Name = "to")]
        public string? To { get; set; }

        [JsonPropertyName("page"), FromQuery(Name = "page")]
        public int Page { get; set; } = 1;

        [JsonPropertyName("page_size"), FromQuery(Name = "page_size")]
        public int PageSize { get; set; } = 10;

        [JsonPropertyName("department"), FromQuery(Name = "department")]
        public Domain.Entities.Department? Department { get; set; }
    }
}