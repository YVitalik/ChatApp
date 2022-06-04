using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.BLL.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int chatId, Message message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
