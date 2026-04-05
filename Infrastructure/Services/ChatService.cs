using Fashion.Api.Core.Entities;
using Fashion.Api.Core.Interfaces;
using Fashion.Api.DTOs;
using Fashion.Api.DTOs.Admin;
using Fashion.Api.DTOs.Chats;
using Fashion.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Fashion.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;

        public ChatService(AppDbContext db, IHttpClientFactory factory)
        {
            _db = db;
            _http = factory.CreateClient();
        }

        // ======================
        // USER SEND MESSAGE
        // ======================
        public async Task<ChatResponseDto> SendMessage(ChatSendRequestDto dto, string firebaseUid)
        {
            Conversation conversation;

            if (dto.ConversationId == null)
            {
                conversation = new Conversation
                {
                    Id = Guid.NewGuid(),
                    FirebaseUid = firebaseUid,
                    Status = "active"
                };

                _db.Conversations.Add(conversation);
            }
            else
            {
                conversation = await _db.Conversations
                    .FirstAsync(c => c.Id == dto.ConversationId);
            }

            // Save user message
            var userMessage = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                Role = "user",
                Content = dto.Message
            };

            _db.Messages.Add(userMessage);

            // AI reply
            var aiReply = await GetAiReply(dto.Message);

            var aiMessage = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
                Role = "ai",
                Content = aiReply
            };

            _db.Messages.Add(aiMessage);

            await _db.SaveChangesAsync();

            return new ChatResponseDto
            {
                ConversationId = conversation.Id,
                AiMessage = new MessageDto
                {
                    Id = aiMessage.Id,
                    Role = aiMessage.Role,
                    Content = aiMessage.Content,
                    CreatedAt = aiMessage.CreatedAt
                }
            };
        }

        // ======================
        // AI RESPONSE
        // ======================
        //private async Task<string> GetAiReply(string message)
        //{
        //    var request = new
        //    {
        //        model = "gpt-4o-mini",
        //        messages = new[]
        //        {
        //            new { role = "user", content = message }
        //        }
        //    };

        //    var content = new StringContent(
        //        JsonSerializer.Serialize(request),
        //        Encoding.UTF8,
        //        "application/json");

        //    _http.DefaultRequestHeaders.Authorization =
        //        new System.Net.Http.Headers.AuthenticationHeaderValue(
        //            "Bearer",
        //            "YOUR_OPENAI_API_KEY");

        //    var response = await _http.PostAsync(
        //        "https://api.openai.com/v1/chat/completions",
        //        content);

        //    var json = await response.Content.ReadAsStringAsync();

        //    using var doc = JsonDocument.Parse(json);

        //    return doc.RootElement
        //        .GetProperty("choices")[0]
        //        .GetProperty("message")
        //        .GetProperty("content")
        //        .GetString();
        //}

        // ======================
        // AI RESPONSE (MOCK)
        // ======================
        private Task<string> GetAiReply(string message)
        {
            string reply;

            if (message.ToLower().Contains("order"))
            {
                reply = "You can check your order status in the Orders section of your account.";
            }
            else if (message.ToLower().Contains("return"))
            {
                reply = "Returns are accepted within 7 days of delivery.";
            }
            else if (message.ToLower().Contains("hello"))
            {
                reply = "Hello! 👋 How can I assist you today?";
            }
            else
            {
                reply = $"Thanks for your message: \"{message}\". Our AI assistant is currently in development.";
            }

            return Task.FromResult(reply);
        }

        // ======================
        // ADMIN: GET CONVERSATIONS
        // ======================
        public async Task<List<AdminConversationDto>> GetAllConversations()
        {
            return await _db.Conversations
                .Select(c => new AdminConversationDto
                {
                    ConversationId = c.Id,
                    FirebaseUid = c.FirebaseUid,
                    LastMessage = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Content)
                        .FirstOrDefault(),

                    LastMessageTime = c.Messages
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.CreatedAt)
                        .FirstOrDefault(),

                    Status = c.Status
                })
                .OrderByDescending(c => c.LastMessageTime)
                .ToListAsync();
        }

        // ======================
        // GET MESSAGES
        // ======================
        public async Task<List<MessageDto>> GetConversationMessages(Guid conversationId)
        {
            return await _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Role = m.Role,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();
        }

        // ======================
        // ADMIN REPLY
        // ======================
        public async Task<MessageDto> AdminReply(AdminReplyDto dto)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = dto.ConversationId,
                Role = "agent",
                Content = dto.Message
            };

            _db.Messages.Add(message);

            await _db.SaveChangesAsync();

            return new MessageDto
            {
                Id = message.Id,
                Role = message.Role,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };
        }
    }
}