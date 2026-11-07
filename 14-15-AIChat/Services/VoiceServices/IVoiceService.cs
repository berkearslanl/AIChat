namespace _14_15_AIChat.Services.VoiceServices
{
    public interface IVoiceService
    {
        Task<byte[]> SynthesizeAsync(string text,CancellationToken cancellationToken);
        Task<string> TranscribeAsync(Stream audio, string fileName, string contentType, CancellationToken cancellationToken);
    }
}
