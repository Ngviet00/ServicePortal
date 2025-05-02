using System.Text.Json.Serialization;

namespace ServicePortal.Modules.LeaveRequest.DTO
{
    public class ApprovalDTO
    {
        [JsonPropertyName("user_code_approval")]
        public string? UserCodeApproval { get; set; }

        [JsonPropertyName("leave_request_id")]
        public string? LeaveRequestId { get; set; }

        [JsonPropertyName("status")]
        public bool Status { get; set; } //true approval, false reject

        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }
}
