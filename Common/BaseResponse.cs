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
}
