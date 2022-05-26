using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateChatService
    {
        Task<bool> CreatePublicChat(string chatName);
    }
}
