namespace Fashion.Api.DTOs.Chats
{
    public class ConversationDto
    {
        public Guid Id { get; set; }

        public string FirebaseUid { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<MessageDto> Messages { get; set; }
    }
}
