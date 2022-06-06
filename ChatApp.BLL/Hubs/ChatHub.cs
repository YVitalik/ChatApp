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

        public async Task SendUpdatedMessage(int chatId, Message message)
        {
            await Clients.Group(chatId.ToString()).SendAsync("ReceiveUpdatedMessage", message);
        }

        public async Task DeleteMessage(int chatId, int messageId)
        {
            await Clients.Group(chatId.ToString()).SendAsync("UpdateChatWhenMessageIdDeleted", messageId);
        }
    }
}
