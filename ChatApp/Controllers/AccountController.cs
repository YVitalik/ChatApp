using ChatApp.BLL.CustomExceptions;
using ChatApp.BLL.DTOs.AdministrationDTOs;
using ChatApp.BLL.Infrastructure.JwtHelper;
using ChatApp.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("chatapi/account")]
    public class AccountController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;

        public AccountController(IUserService userService, IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            try
            {
                var user = await _userService.Login(model);

                if (user is null) return BadRequest("Password is incorrect");
                
                return Ok(new { Token = JwtHelper.GenerateJwt(user, _jwtSettings) });
            }
            catch (UserDoesntExistsException ex)
            {
                return StatusCode(400, ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            try
            {
                await _userService.Register(model);
                return Ok(model);
            }
            catch (UsernameAlreadyExistsException ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
    }
}
