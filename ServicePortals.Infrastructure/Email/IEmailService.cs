namespace ServicePortals.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendEmailAsync(List<string>? to, List<string>? cc, string subject, string? body, List<(string, byte[])>? attachments, bool isHtml = true);
    }
}
