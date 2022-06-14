using System.ComponentModel.DataAnnotations;

namespace ChatApp.BLL.DTOs.ChatDTOs
{
    public class CreateMessageDto
    {
        public int ChatId { get; set; }
        [Required]
        public string MessageText { get; set; }
        public string? Name { get; set; }
        public string? SenderId { get; set; }
    }
}
