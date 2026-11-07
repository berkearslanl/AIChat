using Microsoft.AspNetCore.Mvc;

namespace _14_15_AIChat.ViewComponents
{
    public class MessageComposerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
