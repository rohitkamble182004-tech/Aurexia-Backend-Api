using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs;
using Fashion.Api.DTOs.Chats;
using Fashion.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/chat")]
    [Authorize] // requires JWT
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // =====================
        // USER SEND MESSAGE
        // =====================
        [HttpPost("send")]
        public async Task<ActionResult<ChatResponseDto>> SendMessage(
            [FromBody] ChatSendRequestDto dto)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(firebaseUid))
                return Unauthorized();

            var response = await _chatService.SendMessage(dto, firebaseUid);

            return Ok(response);
        }

        // =====================
        // USER GET MESSAGES
        // =====================
        [HttpGet("messages/{conversationId}")]
        public async Task<ActionResult<List<MessageDto>>> GetMessages(Guid conversationId)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(firebaseUid))
                return Unauthorized();

            var messages = await _chatService.GetConversationMessages(conversationId);

            return Ok(messages);
        }
    }
}