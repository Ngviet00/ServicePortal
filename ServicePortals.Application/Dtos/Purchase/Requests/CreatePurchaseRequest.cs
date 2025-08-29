using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Purchase.Requests
{
    public class CreatePurchaseRequest
    {
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? DepartmentId { get; set; }
        public int? OrgPositionId { get; set; }
        public DateTimeOffset? RequestedDate { get; set; }
        public List<CreatePurchaseDetailRequest> CreatePurchaseDetailRequests { get; set; } = [];
        public string? UrlFrontend { get; set; }
    }
}
