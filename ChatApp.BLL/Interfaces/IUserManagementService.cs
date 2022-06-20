using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<User>> GetAllUsersExceptCurrent(string currentUserId);
        Task<User> GetUserByIdAsync(string userId);
        Task<List<User>> GetUsersForPrivateChats(string rootId, string targetId);
    }
}
