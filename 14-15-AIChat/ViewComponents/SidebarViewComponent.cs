using _14_15_AIChat.Context;
using _14_15_AIChat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _14_15_AIChat.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SidebarViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? selectedId)
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            var fullName = await _context.Users
                                .AsNoTracking()
                                .Where(u => u.Id == userId)
                                .Select(u => u.FullName)
                                .FirstOrDefaultAsync() ?? "";

            var conversations = await _context.Conversations
                                                    .AsNoTracking()
                                                    .Where(x => x.AppUserId == userId)
                                                    .OrderByDescending(x => x.UpdatedAt)
                                                    .ToListAsync();
            var today = DateTime.UtcNow.AddHours(3).Date;
            var groups = conversations
                            .GroupBy(x => GetGroupTitle(x.UpdatedAt.AddHours(3).Date, today))
                            .Select(g => new ConversationGroup
                            {
                                Title = g.Key,
                                Items = g.ToList()
                            }).ToList();
            return View(new SidebarViewModel
            {
                Groups = groups,
                SelectedConversationId = selectedId,
                FullName = fullName
            });
        }
        private static string GetGroupTitle(DateTime date, DateTime today)
        {
            var days = (today - date).Days;

            if (days <= 0) return "Bugün";
            if (days == 1) return "Dün";
            if (days <= 7) return "Önceki 7 gün";
            return "Daha eski";
        }
    }
}
