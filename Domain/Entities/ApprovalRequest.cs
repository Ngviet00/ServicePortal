using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("approval_requests"), Index(nameof(RequesterUserCode), nameof(RequestId), nameof(CurrentPositionId), nameof(RequestType))]
    public class ApprovalRequest
    {
        public Guid? Id { get; set; }
        public string? RequesterUserCode { get; set; } //22757
        public string? RequestType { get; set; } // LEAVE_REQUEST
        public Guid? RequestId { get; set; } //id of leave request, purchase
        public string? Status { get; set; } //pending, approval, reject
        public int? CurrentPositionId { get; set; } //vi tri người duyệt
        public DateTimeOffset? CreatedAt { get; set; }
    }
}
