using ChatApp.Administration.Interfaces;
using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.AdministrationDTOs;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Administration.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserService(UserManager<IdentityUser> userManager,
                           RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityUser> Login(LoginDTO login)
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

            var identityUser = new IdentityUser { UserName = user.Username, Email = user.Email };
            var result = await _userManager.CreateAsync(identityUser, user.Password);

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
