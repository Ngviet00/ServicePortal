namespace ServicePortals.Application.Dtos.Approval.Response
{
    public class CountWaitApprovalAndAssignedInSidebarResponse
    {
        public int Total => TotalWaitApproval + TotalAssigned;
        public int TotalWaitApproval { get; set; } = 0;
        public int TotalAssigned { get; set; } = 0;
    }
}
