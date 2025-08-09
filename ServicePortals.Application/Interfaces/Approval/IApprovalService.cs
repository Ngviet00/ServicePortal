namespace ServicePortals.Application.Interfaces.Approval
{
    public interface IApprovalService
    {
        Task<object> Approval();
        Task<object> ListWaitApproval();
        Task<object> ListAssigned();
        Task<object> ListHistoryApprovedOrProcessed();
    }
}
