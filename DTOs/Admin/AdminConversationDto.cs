namespace Fashion.Api.DTOs.Admin
{
    public class AdminConversationDto
    {
        public Guid ConversationId { get; set; }

        public string FirebaseUid { get; set; }

        public string LastMessage { get; set; }

        public DateTime LastMessageTime { get; set; }

        public string Status { get; set; }
    }
}
