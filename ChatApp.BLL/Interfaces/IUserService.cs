using ChatApp.BLL.DTOs.AdministrationDTOs;
using ChatApp.DAL.Entities;

namespace ChatApp.BLL.Interfaces
{
    public interface IUserService
    {
        Task Register(RegisterDTO user);
        Task<User> Login(LoginDTO login);
    }
}
