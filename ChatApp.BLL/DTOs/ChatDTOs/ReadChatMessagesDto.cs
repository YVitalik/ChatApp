namespace ChatApp.BLL.DTOs.ChatDTOs
{
    public class ReadChatMessagesDto
    {
        public int ChatId { get; set; }
        public int AmountOfMessagesToTake { get; set; }
        public DateTime? TimeOfSending { get; set; }
    }
}
