using System.Net.Http.Headers;
using System.Text.Json;

namespace _14_15_AIChat.Services.VoiceServices
{
    public class VoiceService : IVoiceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _voiceId;
        private readonly string _ttsModel;
        private readonly string _sttModel;
        private readonly string _languageCode;

        public VoiceService(HttpClient httpClient, IConfiguration _configuration)
        {
            _httpClient = httpClient;
            _voiceId = _configuration["ElevenLabs:VoiceId"] ?? throw new InvalidOperationException("ElevenLabs:VoiceId ayarı bulunamadı.");
            _ttsModel = _configuration["ElevenLabs:TtsModel"] ?? "eleven_multilingual_v2";
            _sttModel = _configuration["ElevenLabs:SttModel"] ?? "scribe_v2";
            _languageCode = _configuration["ElevenLabs:LanguageCode"] ?? "tr";
        }

        public async Task<byte[]> SynthesizeAsync(string text, CancellationToken cancellationToken)
        {
            var requestBody = new
            {
                text,
                model_id = _ttsModel
            };

            using var response = await _httpClient.PostAsJsonAsync
                (
                    $"text-to-speech/{_voiceId}?output_format=mp3_44100_128", requestBody, cancellationToken
                );
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"ElevenLabs hatası ({(int)response.StatusCode}): {error}");
            }
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        public async Task<string> TranscribeAsync(Stream audio, string fileName, string contentType, CancellationToken cancellationToken)
        {
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(_sttModel), "model_id");
            form.Add(new StringContent(_languageCode), "language_code");

            var fileContent = new StreamContent(audio);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType.Split(';')[0].Trim());
            form.Add(fileContent, "file", fileName);

            using var response = await _httpClient.PostAsync("speech-to-text", form, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"ElevenLabs STT hatası ({(int)response.StatusCode}): {error}");
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            return doc.RootElement.GetProperty("text").GetString() ?? "";
        }
    }
}