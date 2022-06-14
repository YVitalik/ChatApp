using ChatApp.BLL.Interfaces;
using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.BLL.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        public UserManagementService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<User>> GetAllUsersExceptCurrent(string currentUserId)
        {
            return await _userManager.Users.Where(x => x.Id != currentUserId).ToListAsync();
        }
    }
}
