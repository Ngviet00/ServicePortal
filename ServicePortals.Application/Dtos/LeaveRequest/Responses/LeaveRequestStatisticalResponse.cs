using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Dtos.LeaveRequest.Responses
{
    public class LeaveRequestStatisticalResponse
    {
        public GroupByTotal? GroupByTotal { get; set; }
        public List<GroupRecentList>? GroupRecentList { get; set; }
        public List<GroupByDepartment>? GroupByDepartment { get; set; }
        public List<GroupByMonth>? GroupByMonth { get; set; }
    }

    public class GroupByTotal
    {
        public int Total { get; set; }
        public int InProcess { get; set; }
        public int Complete { get; set; }
        public int Pending { get; set; }
        public int Reject { get; set; }
    }

    public class GroupRecentList
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string UserNameRequestor { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTimeOffset? CreatedAt { get; set; }
        public int? RequestStatusId { get; set; }
        public string RequestStatus { get; set; } = string.Empty;
        public string RequestStatusE { get; set; } = string.Empty;
    }

    public class GroupByDepartment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Total { get; set; }
    }

    public class GroupByMonth
    {
        public int Mon { get; set; }
        public int Total { get; set; }
    }
}
