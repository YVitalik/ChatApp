using ChatApp.BLL.DTOs.AdministrationDTOs;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Administration.Interfaces
{
    public interface IUserService
    {
        Task<string> Register(RegisterDTO user);
        Task<IdentityUser> Login(LoginDTO login);
    }
}
