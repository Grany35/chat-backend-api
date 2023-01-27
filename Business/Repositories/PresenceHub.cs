using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Business.Repositories
{
    [Authorize]
    public class PresenceHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("UserIsOnline", Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserIsOffline", Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await base.OnDisconnectedAsync(exception);
        }
    }
}