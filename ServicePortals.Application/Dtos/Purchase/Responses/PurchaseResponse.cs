using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Dtos.Purchase.Responses
{
    public class PurchaseResponse
    {
        public Guid? Id { get; set; }
        public Guid? ApplicationFormId { get; set; }
        public string? Code { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public int? DepartmentId { get; set; }
        public DateTimeOffset? RequestedDate { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public Domain.Entities.ApplicationForm? ApplicationForm { get; set; }
        public ICollection<PurchaseDetail>? PurchaseDetails { get; set; }
        public Domain.Entities.OrgUnit? OrgUnit { get; set; }
    }
}
