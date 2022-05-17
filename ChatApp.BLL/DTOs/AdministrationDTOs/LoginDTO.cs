using System.ComponentModel.DataAnnotations;

namespace ChatApp.BLL.DTOs.AdministrationDTOs
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
