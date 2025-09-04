namespace ServicePortals.Application.Dtos.Purchase.Requests
{
    public class CreatePurchaseDetailRequest
    {
        public Guid? Id { get; set; }
        public string? ItemName { get; set; }
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; } = 0;
        public string? UnitMeasurement { get; set; }
        public DateTimeOffset? RequiredDate { get; set; }
        public int? CostCenterId { get; set; }
        public string? Note { get; set; }
    }
}
