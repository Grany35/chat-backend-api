using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Business.Hubs
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;

        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }

        public override async Task OnConnectedAsync()
        {
            await _tracker.UserConnected(Convert.ToInt32(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _tracker.UserDisconnected(Convert.ToInt32(Context.User.FindFirst(ClaimTypes.NameIdentifier).Value),
                Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

            await base.OnDisconnectedAsync(exception);
        }
    }
}