using AutoMapper;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsers(string currentUserId)
        {
            return await _userManager.Users.Where(x => x.Id != currentUserId).ToListAsync();
        }

        public async Task<Message> AddMessage(CreateMessageDto messageDto)
        {
            var message = new Message
            {
                ChatId = messageDto.ChatId,
                Text = messageDto.MessageText,
                Name = messageDto.Name,
                SenderId = messageDto.SenderId,
                CreatedAt = DateTime.Now
            };
            
            var messageToReturn = await _unitOfWork.Message.AddMessage(message);
            await _unitOfWork.SaveChangesAsync();

            return messageToReturn;
        }

        public async Task CreatePrivateRoom(string rootId, string targetId)
        {
            var privateChatUserNames = await _userManager.Users.Where(x => x.Id == rootId || x.Id == targetId).ToListAsync();
            await _unitOfWork.Room.CreatePrivateRoom(rootId, targetId, privateChatUserNames);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreatePublicRoom(string name, string userId)
        {
            await _unitOfWork.Room.CreatePublicRoom(name, userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> DeleteMessage(DeleteMessageDto deleteMessageDto)
        {
            var messageFromDb = await _unitOfWork.Message.GetMessage(deleteMessageDto.MessageId);

            if (messageFromDb.SenderId == deleteMessageDto.UserId)
            {
                await _unitOfWork.Message.DeleteMessage(deleteMessageDto.MessageId);
                await _unitOfWork.SaveChangesAsync();
                return deleteMessageDto.MessageId;
            }
            else
            {
                throw new InvalidUserException("You can delete only your own messages!");
            }
        }

        public async Task<IEnumerable<ReadChatDto>> GetAllPublicChats(string userId)
        {
            var chats = await _unitOfWork.Room.GetAllPublicChats(userId);
            return _mapper.Map<IEnumerable<ReadChatDto>>(chats);
        }

        public async Task<IEnumerable<Message>> GetChatMessages(int chatId)
        {
            return await _unitOfWork.Message.GetChatMessages(chatId);
            //return _mapper.Map<IEnumerable<ReadMessageDto>>(result);
        }

        public async Task<IEnumerable<Chat>> GetPrivateChats(string userId)
        {
            return await _unitOfWork.Room.GetPrivateChats(userId);
        }

        public async Task<IEnumerable<ReadChatDto>> GetUserPublicChats(string userId)
        {
            var result = await _unitOfWork.Room.GetUserPublicChats(userId);
            return _mapper.Map<IEnumerable<ReadChatDto>>(result);
        }

        public async Task JoinRoom(int chatId, string userId)
        {
            await _unitOfWork.Room.JoinRoom(chatId, userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<Message> UpdateMessage(UpdateMessageDto updateMessageDto)
        {
            var messageFromDb = await _unitOfWork.Message.GetMessage(updateMessageDto.Id);
            
            if (messageFromDb != null)
            {
                if (messageFromDb.SenderId == updateMessageDto.SenderId)
                {
                    messageFromDb.Text = updateMessageDto.Text;
                    var messageToReturn = _unitOfWork.Message.UpdateMessage(messageFromDb);
                    
                    await _unitOfWork.SaveChangesAsync();

                    return messageToReturn; 
                }
                else
                {
                    throw new InvalidUserException("You can edit and delete only your own messages!");
                }
            }
            else
            {
                throw new ArgumentNullException("Message with such id doesnt exist!");
            }
        }

        public async Task<Message> ReplyMessage(ReplyMessageDto replyMessageDto)
        {
            var chat = await _unitOfWork.Room.GetChatByName(replyMessageDto.ChatNameToReply);
            var message = await _unitOfWork.Message.GetMessage(replyMessageDto.MessageId);
            var messageSenderName = await _userManager.FindByIdAsync(message.SenderId);

            var repliedMessage = new Message();

            if (message.Name == "Replied from: " + messageSenderName.UserName)
            {
                repliedMessage = new Message
                {
                    ChatId = chat.Id,
                    Text = message.Text,
                    Name = message.Name,
                    SenderId = message.SenderId,
                    CreatedAt = DateTime.Now
                };
            }
            
            else
            {
                repliedMessage = new Message
                {
                    ChatId = chat.Id,
                    Text = message.Text,
                    Name = "Replied from: " + message.Name,
                    SenderId = message.SenderId,
                    CreatedAt = DateTime.Now
                };
            }

            var messageToReturn = await _unitOfWork.Message.AddMessage(repliedMessage);
            await _unitOfWork.SaveChangesAsync();

            return messageToReturn;
        }
    }
}
