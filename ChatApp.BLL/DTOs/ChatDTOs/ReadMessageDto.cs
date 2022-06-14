namespace ChatApp.BLL.DTOs.ChatDTOs
{
    public class ReadMessageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ChatId { get; set; }
    }
}
