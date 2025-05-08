using Microsoft.AspNetCore.SignalR;

namespace ServicePortal.Infrastructure.Hubs
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendMessageToUser(string userCode, string message)
        {
            await _hubContext.Clients.Group(userCode).SendAsync("login_again", message);
        }
    }
}
