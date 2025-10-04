namespace ServicePortals.Application.Dtos.ITForm.Responses
{
    public class StatisticalFormITResponse
    {
        public GroupByTotal? GroupByTotal { get; set; }
        public List<GroupRecentList>? GroupRecentList { get; set; }
        public List<GroupByDepartment>? GroupByDepartment { get; set; }
        public List<GroupByMonth>? GroupByMonth { get; set; }
        public List<GroupByCategory>? GroupByCategory { get; set; }
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
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public DateTimeOffset? CreatedAt { get; set; }
        public string NamePriority { get; set; } = string.Empty;
        public string NamePriorityE { get; set; } = string.Empty;
        public int? RequestStatusId { get; set; }
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

    public class GroupByCategory
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Total { get; set; }
    }
}
