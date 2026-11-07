using _14_15_AIChat.Services.VoiceServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _14_15_AIChat.Controllers
{
    [Route("api/voice")]
    [ApiController]
    [Authorize]
    public class VoiceController : ControllerBase
    {
        private const int MaxTextLength = 3000;
        private const long MaxAudioBytes = 10 * 1024 * 1024;

        private readonly IVoiceService _voiceService;
        private readonly ILogger<VoiceController> _logger;

        public VoiceController(IVoiceService voiceService, ILogger<VoiceController> logger)
        {
            _voiceService = voiceService;
            _logger = logger;
        }
        public record SpeakRequest(string Text);

        [HttpPost("speak")]
        public async Task<IActionResult> Speak([FromBody] SpeakRequest request, CancellationToken cancellationToken)
        {
            var text = request.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Metin boş olamaz");

            if (text.Length > MaxTextLength)
                text = text[..MaxTextLength];

            try
            {
                var audio = await _voiceService.SynthesizeAsync(text, cancellationToken);
                return File(audio, "audio/mpeg");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Ses üretimi başarısız.");
                return StatusCode(StatusCodes.Status502BadGateway, "Ses servisi şuan kullanılamıyor.");
            }
        }
        [HttpPost("transcribe")]
        public async Task<IActionResult> Transcribe(IFormFile audio, CancellationToken cancellationToken)
        {
            if (audio is null || audio.Length == 0)
                return BadRequest("Ses dosyası boş.");

            if (audio.Length > MaxAudioBytes)
                return BadRequest("Ses dosyası çok büyük.");

            try
            {
                await using var stream = audio.OpenReadStream();
                var text = await _voiceService.TranscribeAsync(stream, audio.FileName, audio.ContentType, cancellationToken);
                return Ok(new { text });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Ses tanıma başarısız.");
                return StatusCode(StatusCodes.Status502BadGateway, "Ses tanıma servisi şu an kullanılamıyor.");
            }
        }
    }
}