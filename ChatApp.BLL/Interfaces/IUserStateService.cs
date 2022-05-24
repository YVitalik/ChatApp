using ChatApp.BLL.DTOs.AdministrationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.Interfaces
{
    public interface IUserStateService
    {
        Task<string> Login(LoginDTO loginDetails);
        Task<string> Register(RegisterDTO registerDetails);
    }
}
