using Fashion.Api.DTOs;
using Fashion.Api.DTOs.Admin;
using Fashion.Api.DTOs.Chats;

namespace Fashion.Api.Services
{
    public interface IChatService
    {
        Task<ChatResponseDto> SendMessage(ChatSendRequestDto dto, string firebaseUid);

        Task<List<AdminConversationDto>> GetAllConversations();

        Task<List<MessageDto>> GetConversationMessages(Guid conversationId);

        Task<MessageDto> AdminReply(AdminReplyDto dto);
    }
}