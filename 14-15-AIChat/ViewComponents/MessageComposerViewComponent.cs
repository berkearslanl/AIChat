using Microsoft.AspNetCore.Mvc;

namespace _14_15_AIChat.ViewComponents
{
    // Mesaj yazma kutusu: öneri butonları, mikrofon ve gönder butonu.
    public class MessageComposerViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // View() -> Views/Shared/Components/MessageComposer/Default.cshtml
            return View();
        }
    }
}
