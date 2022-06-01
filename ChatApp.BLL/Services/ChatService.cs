﻿using AutoMapper;
using ChatApp.BLL.DTOs.ChatDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;

namespace ChatApp.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddMessage(int chatId, string messageText, string userId)
        {
            await _unitOfWork.Message.AddMessage(chatId, messageText, userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreatePrivateRoom(string rootId, string targetId)
        {
            await _unitOfWork.Room.CreatePrivateRoom(rootId, targetId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreatePublicRoom(string name, string userId)
        {
            await _unitOfWork.Room.CreatePublicRoom(name, userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReadChatDto>> GetAllPublicChats(string userId)
        {
            var chats = await _unitOfWork.Room.GetAllPublicChats(userId);
            return _mapper.Map<IEnumerable<ReadChatDto>>(chats);
        }

        public async Task<IEnumerable<ReadMessageDto>> GetChatMessages(int chatId)
        {
            var result = await _unitOfWork.Message.GetChatMessages(chatId);
            return _mapper.Map<IEnumerable<ReadMessageDto>>(result);
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
    }
}