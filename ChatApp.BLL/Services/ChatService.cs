using ChatApp.BLL.Interfaces;
using ChatApp.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ChatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreatePublicRoom(string name, string userId)
        {
            await _unitOfWork.Room.CreatePublicRoom(name, userId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
