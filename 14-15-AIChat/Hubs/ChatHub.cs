using _14_15_AIChat.Context;
using _14_15_AIChat.Entities;
using _14_15_AIChat.Enums;
using _14_15_AIChat.Services.ChatServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace _14_15_AIChat.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly IAiChatService _aiChatService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(AppDbContext context, IAiChatService aiChatService, ILogger<ChatHub> logger)
        {
            _context = context;
            _aiChatService = aiChatService;
            _logger = logger;
        }

        public async Task SendMessage(int? conversationId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            message = message.Trim();
            var userId = Context.UserIdentifier;
            if (userId is null)
            {
                await Clients.Caller.SendAsync("ReceiveError", "Oturum bulunamadı.");
                return;
            }
            var ct = Context.ConnectionAborted;
            try
            {
                var conversation = conversationId is null
                    ? await CreateConversationAsync(message, userId, ct)
                    : await _context.Conversations.FirstOrDefaultAsync(c => c.ConversationId == conversationId && c.AppUserId==userId, ct);

                if (conversation == null)
                {
                    await Clients.Caller.SendAsync("ReceiveError", "Sohbet bulunamadı.", ct);
                    return;
                }

                _context.Messages.Add(new Message
                {
                    ConversationId = conversation.ConversationId,
                    MessageRole = MessageRole.User,
                    Content = message,
                    CreatedAt = DateTime.UtcNow
                });

                conversation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);

                var history = await _context.Messages
                    .AsNoTracking()
                    .Where(h => h.ConversationId == conversation.ConversationId)
                    .OrderBy(h => h.CreatedAt)
                    .ToListAsync(ct);

                var reply = new StringBuilder();

                await foreach (var chunk in _aiChatService.StreamResponseAsync(history, ct))
                {
                    reply.Append(chunk);
                    await Clients.Caller.SendAsync("ReceiveChunk", chunk, ct);
                }

                if (reply.Length == 0)
                {
                    await Clients.Caller.SendAsync("ReceiveError", "AI boş bir cevap döndürdü.");
                    return;
                }

                _context.Messages.Add(new Message
                {
                    ConversationId = conversation.ConversationId,
                    MessageRole = MessageRole.Assistant,
                    Content = reply.ToString(),
                    CreatedAt = DateTime.UtcNow
                });
                conversation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);

                await Clients.Caller.SendAsync("ReceiveComplete", new
                {
                    conversationId = conversation.ConversationId,
                    isNewConversation = conversationId is null
                }, ct);
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mesaj işlenirken hata oluştu.");
                await Clients.Caller.SendAsync("ReceiveError", "Cevap alınamadı, lütfen tekrar deneyin.");
            }
        }
        private async Task<Conversation> CreateConversationAsync(string firstMessage,string userId, CancellationToken cancellationToken)
        {
            var ct = Context.ConnectionAborted;
            var conversation = new Conversation
            {
                Title = firstMessage.Length > 40 ? firstMessage[..40] + "..." : firstMessage,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AppUserId = userId
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync(ct);
            return conversation;
        }
    }
}