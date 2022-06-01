namespace ChatApp.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomRepository Room { get; }
        IMessageRepository Message { get; }
        Task<int> SaveChangesAsync();
    }
}
