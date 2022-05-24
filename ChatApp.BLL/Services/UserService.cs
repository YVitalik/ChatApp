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
            var user = _userManager.Users.FirstOrDefault(x => x.UserName == login.Username);

            if (user != null)
            {
                return await _userManager.CheckPasswordAsync(user, login.Password) ? user : null;
            }
            else
            {
                throw new UserDoesntExistsException("Username or password is incorrect!");
            }
        }

        public async Task<string> Register(RegisterDTO user)
        {
            var check = _userManager.Users.FirstOrDefault(x => x.UserName == user.Username || x.Email == user.Email);
            
            if (check != null)
            {
                throw new UsernameAlreadyExistsException("Username is already in use, please choose other!");
            }

            var newUser = new User { UserName = user.Username, Email = user.Email };
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
            {
                return "User has been created succesfully!";
            }

            else
            {
                return "Oops something goes wrong!";
            }
        }
    }
}
