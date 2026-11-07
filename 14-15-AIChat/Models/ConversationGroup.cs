using _14_15_AIChat.Entities;

namespace _14_15_AIChat.Models
{
    public class ConversationGroup
    {
        public string Title { get; set; } = "";
        public List<Conversation> Items { get; set; } = new();
    }
}
