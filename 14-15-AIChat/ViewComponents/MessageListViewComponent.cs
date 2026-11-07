using _14_15_AIChat.Context;
using _14_15_AIChat.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _14_15_AIChat.ViewComponents
{
    public class MessageListViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public MessageListViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? conversationId)
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (conversationId == null)
                return View(new List<Message>());

            var messages = await _context.Messages
                                    .AsNoTracking()
                                    .Where(x => x.ConversationId == conversationId && x.Conversation.AppUserId == userId)
                                    .OrderBy(t => t.CreatedAt)
                                    .ToListAsync();
            return View(messages);
        }
    }
}
