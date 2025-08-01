namespace ServicePortals.Application.Dtos.TimeKeeping.Requests
{
    public class GetListHistoryEditTimeKeepingRequest
    {
        public string? UserCodeUpdated { get; set; } 

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
