using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs;
using Fashion.Api.DTOs.Admin;
using Fashion.Api.DTOs.Chats;
using Fashion.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fashion.Api.Controllers
{
    [ApiController]
    [Route("api/admin/chat")]
    [Authorize(Roles = "admin")]
    public class AdminChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public AdminChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // ============================
        // GET ALL CONVERSATIONS
        // ============================
        [HttpGet("conversations")]
        public async Task<ActionResult<List<AdminConversationDto>>> GetConversations()
        {
            var conversations = await _chatService.GetAllConversations();

            return Ok(conversations);
        }

        // ============================
        // GET CONVERSATION MESSAGES
        // ============================
        [HttpGet("messages/{conversationId:guid}")]
        public async Task<ActionResult<List<MessageDto>>> GetMessages(Guid conversationId)
        {
            if (conversationId == Guid.Empty)
                return BadRequest("Invalid conversation id.");

            var messages = await _chatService.GetConversationMessages(conversationId);

            return Ok(messages);
        }

        // ============================
        // ADMIN REPLY
        // ============================
        [HttpPost("reply")]
        public async Task<ActionResult<MessageDto>> Reply([FromBody] AdminReplyDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid request body.");

            if (dto.ConversationId == Guid.Empty)
                return BadRequest("ConversationId is required.");

            if (string.IsNullOrWhiteSpace(dto.Message))
                return BadRequest("Message cannot be empty.");

            var message = await _chatService.AdminReply(dto);

            return Ok(message);
        }
    }
}