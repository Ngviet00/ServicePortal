namespace ServicePortal.Common
{
    public class HttpException : Exception
    {
        public int StatusCode { get; }

        public HttpException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class BadRequestException : HttpException
    {
        public BadRequestException(string message) : base(400, message) { }
    }

    public class UnauthorizedException : HttpException
    {
        public UnauthorizedException(string message) : base(401, message) { }
    }

    public class ForbiddenException : HttpException
    {
        public ForbiddenException(string message) : base(403, message) { }
    }

    public class NotFoundException : HttpException
    {
        public NotFoundException(string message) : base(404, message) { }
    }

    public class ValidationException : HttpException
    {
        public ValidationException(string message) : base(422, message) { }
    }
}
