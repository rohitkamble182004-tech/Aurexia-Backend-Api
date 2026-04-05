namespace Fashion.Api.DTOs.Chats
{
    public class ChatResponseDto
    {
        public Guid ConversationId { get; set; }

        public MessageDto AiMessage { get; set; }
    }
}
