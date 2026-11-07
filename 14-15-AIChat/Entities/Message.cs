using _14_15_AIChat.Enums;

namespace _14_15_AIChat.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public MessageRole MessageRole { get; set; }//kullanıcı mı yapay zeka mı
        public string Content { get; set; }//mesaj
        public DateTime CreatedAt { get; set; }//gönderilme zamanı
    }
}