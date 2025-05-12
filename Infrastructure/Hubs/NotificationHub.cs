using Microsoft.AspNetCore.SignalR;

namespace ServicePortal.Infrastructure.Hubs
{
    public class NotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userCode = Context.User?.FindFirst("user_code")?.Value;

            if (userCode != null)
            {
                return Groups.AddToGroupAsync(Context.ConnectionId, userCode);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userCode = Context.User?.FindFirst("user_code")?.Value;

            if (userCode != null)
            {
                return Groups.RemoveFromGroupAsync(Context.ConnectionId, userCode);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToUser(string userCode, string message)
        {
            await Clients.Group(userCode).SendAsync("login_again", message);
        }
    }
}
