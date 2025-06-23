using System.Text.Json;
using Serilog;
using ServicePortals.Shared.Exceptions;

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
            int statusCode;
            string message;

            if (ex is HttpException httpEx)
            {
                statusCode = httpEx.StatusCode;
                message = httpEx.Message;

                if (statusCode >= 500)
                {
                    Log.Error(httpEx, "HttpException caught in middleware");
                }
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "System error. Please try again later.";

                Log.Error(ex, "Unhandled exception caught in middleware");
            }

            var response = new
            {
                status = statusCode,
                message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
