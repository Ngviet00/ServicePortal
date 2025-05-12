using System.Text.Json.Serialization;

namespace ServicePortal.Modules.LeaveRequest.DTO
{
    public class LeaveRequestDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("user_code")]
        public string? UserCode { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("user_code_register")]
        public string? UserCodeRegister { get; set; }

        [JsonPropertyName("name_register")]
        public string? NameRegister { get; set; }

        [JsonPropertyName("position")]
        public string? Position { get; set; }

        [JsonPropertyName("department")]
        public string? Deparment { get; set; }

        [JsonPropertyName("from_date")]
        public string? FromDate { get; set; }

        [JsonPropertyName("to_date")]
        public string? ToDate { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("time_leave")]
        public byte? TimeLeave { get; set; }

        [JsonPropertyName("type_leave")]
        public byte? TypeLeave { get; set; }

        [JsonPropertyName("status")]
        public byte? Status { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("have_salary")]
        public bool? HaveSalary { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        [JsonPropertyName("approved_by")]
        public string? ApprovedBy { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        [JsonPropertyName("url_front_end")]
        public string? UrlFrontEnd { get; set; }
    }
}