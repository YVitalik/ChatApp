using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.Interfaces
{
    public interface IChatService
    {
        Task CreatePublicRoom(string name, string userId);
    }
}
