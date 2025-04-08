using System.Net;
using System.Text.Json;
using ServicePortal.Common;
using ServicePortal.Common.Helpers;
using ServicePortal.Domain.Enums;

namespace ServicePortal.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode = ex is HttpException httpException
                ? httpException.StatusCode
                : (int)HttpStatusCode.InternalServerError;

            string message = ex.Message ?? "System error. Please try again!";

            FileHelper.WriteLog(TypeErrorEnum.ERROR, $"Message: {ex?.Message}\nStackTrace: {ex?.StackTrace}\n");

            var response = new
            {
                StatusCode = statusCode,
                Message = message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
