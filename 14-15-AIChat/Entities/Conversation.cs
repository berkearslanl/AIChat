namespace _14_15_AIChat.Entities
{
    public class Conversation
    {
        public int ConversationId { get; set; }//sohbet id
        public string Title { get; set; }//sohbet başlığı
        public DateTime CreatedAt { get; set; }//oluşturulma zamanı
        public DateTime UpdatedAt { get; set; }//sidebar'da sıralama yapmak için
        public List<Message> Messages { get; set; }

        public string AppUserId { get; set; } = "";
        public AppUser AppUser { get; set; } = null!;
    }
}
