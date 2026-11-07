namespace _14_15_AIChat.Models
{
    public class SidebarViewModel
    {
        public List<ConversationGroup> Groups { get; set; } = new();
        public int? SelectedConversationId { get; set; }
        public string FullName { get; set; } = "";
    }
}