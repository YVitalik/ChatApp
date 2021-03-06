using ChatApp.DAL.Interfaces;

namespace ChatApp.DAL.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRoomRepository Room { get; }
        public IMessageRepository Message { get; }
        public UnitOfWork(AppDbContext context, IRoomRepository room, IMessageRepository message)
        {
            _context = context;
            Room = room;
            Message = message;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
