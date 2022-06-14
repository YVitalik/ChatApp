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

        public async Task CreatePrivateRoom(string rootId, string targetId)
        {
            var privateChatUserNames = await _userManager.Users.Where(x => x.Id == rootId || x.Id == targetId).ToListAsync();
            var checkIfChatExists = await _unitOfWork.Room.GetChatByName(privateChatUserNames[0] + " and " + privateChatUserNames[1] + " private chat");

            if (checkIfChatExists != null)
            {
                throw new ItemWithSuchNameAlreadyExists("You have already got chat with this user!");
            }

            await _unitOfWork.Room.CreatePrivateRoom(rootId, targetId, privateChatUserNames);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreatePublicRoom(string name, string userId)
        {
            var checkIfChatWithSuchNameExists = await _unitOfWork.Room.GetChatByName(name);

            if (checkIfChatWithSuchNameExists != null)
            {
                throw new ItemWithSuchNameAlreadyExists("Chat with such name already exist please choose another!");
            }

            await _unitOfWork.Room.CreatePublicRoom(name, userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReadChatDto>> GetAllPublicChats(string userId)
        {
            var chats = await _unitOfWork.Room.GetAllPublicChats(userId);
            return _mapper.Map<IEnumerable<ReadChatDto>>(chats);
        }

        public async Task<IEnumerable<ReadChatDto>> GetPrivateChats(string userId)
        {
            var privateChats = await _unitOfWork.Room.GetPrivateChats(userId);
            return _mapper.Map<IEnumerable<ReadChatDto>>(privateChats);
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
    }
}
