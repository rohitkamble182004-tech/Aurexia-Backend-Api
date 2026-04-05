using FirebaseAdmin.Messaging;
using System.ComponentModel.DataAnnotations;

namespace Fashion.Api.Core.Entities
{
    public class Conversation
    {
        public Guid Id { get; set; }

        public string FirebaseUid { get; set; }

        public string Status { get; set; } = "active";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; }
    }
}
