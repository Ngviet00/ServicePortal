using System.Text.Json.Serialization;

namespace ServicePortal.Common
{
    public class BaseResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public BaseResponse(int status, string message, T? data)
        {
            Status = status;
            Message = message;
            Data = data;
        }
    }

    public class PageResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<T>? Data { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }
        
        [JsonPropertyName("total_items")]
        public int TotalItems { get; set; }

        [JsonPropertyName("count_pending")]
        public int? CountPending { get; set; }

        [JsonPropertyName("count_in_process")]
        public int? CountInProcess { get; set; }


        public PageResponse(int status, string message, List<T>? data, int totalPage, int currentPage, int perPage, int totalItem, int? countPending = null, int? countInProcess = null)
        {
            Status = status;
            Message = message;
            Data = data;
            TotalPages = totalPage;
            CurrentPage = currentPage;
            PerPage = perPage;
            TotalItems = totalItem;
            CountPending = countPending;
            CountInProcess = countInProcess;
        }
    }

    public class PagedResults<T>
    {
        public List<T>? Data { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int? CountPending { get; set; }
        public int? CountInProcess { get; set; }
    }
}
