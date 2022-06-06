using System.ComponentModel.DataAnnotations;

namespace ChatApp.BLL.DTOs.ChatDTOs
{
    public class UpdateMessageDto
    {
        public int Id { get; set; }
        public string? SenderId { get; set; }
        [Required]
        public string Text { get; set; }
    }
}
