namespace Fashion.Api.DTOs.Chats
{
    public class ChatSendRequestDto
    {
        public Guid? ConversationId { get; set; }

        public string Message { get; set; }
    }
}
