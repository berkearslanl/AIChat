using _14_15_AIChat.Entities;
using _14_15_AIChat.Enums;
using System.Text.Json;

namespace _14_15_AIChat.Services.ChatServices
{
    public class AiChatService : IAiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _model;

        public AiChatService(HttpClient httpClient, IConfiguration _configuration)
        {
            _httpClient = httpClient;
            _model = _configuration["OpenAI:Model"] ?? throw new InvalidOperationException("OpenAI:Model ayarı bulunamadı.");
        }

        public async Task<string> GetResponseAsync(IEnumerable<Message> history, CancellationToken cancellationToken)
        {
            var requestBody = new
            {
                model = _model,
                input = history.Select(x => new
                {
                    role = x.MessageRole == MessageRole.User ? "user" : "assistant",
                    content = x.Content
                }),
                store = false
            };

            using var response = await _httpClient.PostAsJsonAsync("responses", requestBody, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"OpenAI hatası ({(int)response.StatusCode}): {error}");
            }
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            foreach (var item in doc.RootElement.GetProperty("output").EnumerateArray())
            {
                if (item.GetProperty("type").GetString() != "message") continue;

                foreach (var part in item.GetProperty("content").EnumerateArray())
                {
                    if (part.GetProperty("type").GetString() == "output_text")
                        return part.GetProperty("text").GetString() ?? string.Empty;
                }
            }
            throw new InvalidOperationException("OpenAI cevabında metin bulunamadı.");
        }

        public async IAsyncEnumerable<string> StreamResponseAsync(IEnumerable<Message> history, CancellationToken cancellationToken)
        {
            var requestBody = new
            {
                model = _model,
                input = history.Select(m => new
                {
                    role = m.MessageRole == MessageRole.User ? "user" : "assistant",
                    content = m.Content
                }),
                store = false,
                stream = true
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "responses")
            {
                Content = JsonContent.Create(requestBody)
            };

            using var response = await _httpClient.SendAsync(
                request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException($"OpenAI hatası ({(int)response.StatusCode}): {error}");
            }
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            string? eventName = null;

            while (await reader.ReadLineAsync(cancellationToken) is { } line)
            {
                if (line.StartsWith("event:"))
                {
                    eventName = line["event:".Length..].Trim();
                }
                else if (line.StartsWith("data:"))
                {
                    var json = line["data:".Length..].Trim();

                    if (eventName == "response.output_text.delta")
                    {
                        using var doc = JsonDocument.Parse(json);
                        var delta = doc.RootElement.GetProperty("delta").GetString();
                        if (!string.IsNullOrEmpty(delta))
                            yield return delta;
                    }
                    else if (eventName == "response.completed")
                    {
                        yield break;
                    }
                    else if (eventName is "error" or "response.failed")
                    {
                        throw new InvalidOperationException($"OpenAI akış hatası: {json}");
                    }
                }
                else if (line.Length == 0)
                {
                    eventName = null;
                }
            }
        }
    }
}
