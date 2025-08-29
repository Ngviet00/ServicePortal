namespace ServicePortals.Application.Dtos.Purchase.Requests
{
    public class GetAllPurchaseRequest
    {
        public string? UserCode { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? DepartmentId { get; set; }
        public int? RequestStatusId { get; set; }
        public int? Year { get; set; }
    }
}
