using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.DAL.Interfaces
{
    public interface IRoomRepository
    {
        Task CreatePublicRoom(string name, string userId);
    }
}
