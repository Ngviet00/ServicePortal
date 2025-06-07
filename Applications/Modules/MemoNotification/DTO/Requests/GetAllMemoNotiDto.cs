namespace ServicePortal.Applications.Modules.MemoNotification.DTO.Requests
{
    public class GetAllMemoNotiDto
    {
        public int? CreatedByDepartmentId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
