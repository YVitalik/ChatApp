using ChatApp.BLL.DTOs.AdministrationDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.BLL.Interfaces
{
    public interface IIntermediateUserService
    {
        Task<HttpResponseMessage> Login(LoginDTO loginDetails);
        Task<HttpResponseMessage> Register(RegisterDTO registerDetails);
        Task Logout();
    }
}
