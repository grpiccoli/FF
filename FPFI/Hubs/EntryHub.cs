using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FPFI.Hubs
{
    public class EntryHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        public async Task Send(string userId, string function, object message)
        {
            await Clients.User(userId).SendAsync("Update", function, message);
        }
    }
}
