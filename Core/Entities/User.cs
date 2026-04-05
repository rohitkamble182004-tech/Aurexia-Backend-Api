using Fashion.Api.Core.Enums;

namespace Fashion.Api.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        // 🔐 Firebase unique identifier (REQUIRED)
        public string FirebaseUid { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; } = UserRole.User;
    }
}
