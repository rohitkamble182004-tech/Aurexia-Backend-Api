using System.ComponentModel.DataAnnotations;

namespace Fashion.Api.Core.Entities
{
    public class Message
    {
        public Guid Id { get; set; }

        public Guid ConversationId { get; set; }

        public string Role { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public Conversation Conversation { get; set; }
    }
}
