using _14_15_AIChat.Models;
using Microsoft.AspNetCore.Mvc;

namespace _14_15_AIChat.ViewComponents
{
    public class ChatHeaderViewComponent : ViewComponent
    {
        private readonly IConfiguration _configuration;

        public ChatHeaderViewComponent(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IViewComponentResult Invoke()
        {
            return View(new ChatHeaderViewModel
            {
                ModelName = _configuration["OpenAI:Model"] ?? "Bilinmiyor"
            });
        }
    }
}
