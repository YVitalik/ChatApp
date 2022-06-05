using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.BLL.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(int chatId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());
        }

        public async Task SendMessage(int chatId, Message message)
        {
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message);
        }
    }
}
