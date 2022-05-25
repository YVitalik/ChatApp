using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomRepository Room { get; }
        Task<int> SaveChangesAsync();
    }
}
