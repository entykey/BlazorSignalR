namespace BlazorSignalR.Server.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;


    public class BroadcastHub : Hub
    {
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage");
        }
    }
}
