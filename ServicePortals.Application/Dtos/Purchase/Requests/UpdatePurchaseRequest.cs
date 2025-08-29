namespace ServicePortals.Application.Dtos.Purchase.Requests
{
    public class UpdatePurchaseRequest
    {
        public DateTimeOffset? RequestedDate { get; set; }
        public List<CreatePurchaseDetailRequest> CreatePurchaseDetailRequests { get; set; } = [];
    }
}
