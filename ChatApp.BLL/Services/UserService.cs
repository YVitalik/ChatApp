using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.AdministrationDTOs;
using ChatApp.BLL.Interfaces;
using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> Login(LoginDTO login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);

            if (user is null)
            {
                throw new UserDoesntExistsException("Username is incorrect!");
            }
            
            return await _userManager.CheckPasswordAsync(user, login.Password) ? user : null;
        }

        public async Task Register(RegisterDTO user)
        {
            var check = await _userManager.FindByNameAsync(user.Username);
            
            if (check != null)
            {
                throw new UsernameAlreadyExistsException("Username is already in use, please choose other!");
            }

            var newUser = new User { UserName = user.Username, Email = user.Email };
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                throw new ArgumentException("Oops something went wrong please try again!");
            }
        }
    }
}
