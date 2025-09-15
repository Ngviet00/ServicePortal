namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class MyLeaveRequestResponse
    {
        public Guid? LeaveRequestId { get; set; } // LR.Id
        public string? UserCode { get; set; } // LR.UserCode
        public string? UserName { get; set; } // LR.UserName
        public string? DepartmentName { get; set; } // OU.Name
        public string? Position { get; set; } // LR.Position
        public string? TypeLeaveName { get; set; } // TypeLeave.Name
        public string? TypeLeaveNameE { get; set; } // TypeLeave.NameE
        public string? TimeLeaveName { get; set; } // TimeLeave.Name
        public string? TimeLeaveNameE { get; set; } // TimeLeave.NameE
        public DateTimeOffset? FromDate { get; set; } // LR.FromDate
        public DateTimeOffset? ToDate { get; set; } // LR.ToDate
        public string? Reason { get; set; } // LR.Reason
        public DateTimeOffset? CreatedAt { get; set; } // LR.CreatedAt
        public string? CreatedBy { get; set; } // AF.CreatedBy
        public int? RequestStatusId { get; set; } // AF.RequestStatusId
        public string? RequestStatusName { get; set; } // RS.Name
        public string? RequestStatusNameE { get; set; } // RS.NameE
        public string? UserCodeCreatedBy { get; set; }
        public string? Code { get; set; }
    }
}
