﻿using ChatApp.DAL.Entities;
using ChatApp.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DAL.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddMessage(int chatId, string messageText, string userId)
        {
            var message = new Message
            {
                ChatId = chatId,
                Text = messageText,
                Name = userId,
                CreatedAt = DateTime.Now
            };

            await _context.Messages.AddAsync(message);
        }

        public async Task<IEnumerable<Message>> GetChatMessages(int chatId)
        {
            return await _context.Messages.Where(x => x.ChatId == chatId).ToListAsync();
        }
    }
}
