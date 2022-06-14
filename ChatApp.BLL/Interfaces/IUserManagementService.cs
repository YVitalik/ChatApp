using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<User>> GetAllUsersExceptCurrent(string currentUserId);
    }
}
