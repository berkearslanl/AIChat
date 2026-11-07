using _14_15_AIChat.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace _14_15_AIChat.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public IActionResult SideBar(int? id)
        {
            return ViewComponent("Sidebar", new { selectedId = id });
        }

        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                var ownsIt = await _context.Conversations
                    .AnyAsync(c => c.ConversationId == id && c.AppUserId == UserId);

                if (!ownsIt)
                    return RedirectToAction("Index", "Home");
            }
            return View(id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConversation(int id)
        {
            var value = await _context.Conversations.FirstOrDefaultAsync(x => x.ConversationId == id && x.AppUserId == UserId);
            if (value == null)
                throw new KeyNotFoundException($"{id} numaralı sohbet bulunamadı.");

            _context.Conversations.Remove(value);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenameConversation(int id, string title)
        {
            var value = await _context.Conversations.FirstOrDefaultAsync(x => x.ConversationId == id && x.AppUserId == UserId);
            if (value == null)
                throw new KeyNotFoundException($"{id} numaralı sohbet bulunamadı.");

            title = title?.Trim();

            if (string.IsNullOrWhiteSpace(title))
                return RedirectToAction("Index", "Home", new { id });

            if (title.Length > 100)
                title = title[..100];

            value.Title = title;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { id });
        }
    }
}