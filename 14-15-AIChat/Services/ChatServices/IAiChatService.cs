using _14_15_AIChat.Entities;

namespace _14_15_AIChat.Services.ChatServices
{
    public interface IAiChatService
    {
        Task<string> GetResponseAsync(IEnumerable<Message> history, CancellationToken cancellationToken);
        IAsyncEnumerable<string> StreamResponseAsync(IEnumerable<Message> history, CancellationToken cancellationToken);
    }
}