
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeatherApp.Services;

[EnableCors("AllowBlazorWasm")]
[Route("api/geminichat")]
[ApiController]
public class GeminiChatController : ControllerBase
{
    private readonly GeminiChatService _geminiChatService;

    public GeminiChatController(GeminiChatService geminiChatService)
    {
        _geminiChatService = geminiChatService ?? throw new ArgumentNullException(nameof(geminiChatService));
    }

    [HttpPost]
    public async Task<IActionResult> GetChatbotResponse([FromBody] ChatRequestt request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message cannot be empty." });
        }

        try
        {
            var chatResponse = await _geminiChatService.GetGeminiResponseAsync(request.Message);
            return Ok(new ChatResponsee { Response = chatResponse });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GeminiChatController: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while processing the request." });
        }
    }
}

// DTO for receiving user message
public class ChatRequestt
{
    public string Message { get; set; } = string.Empty;
}

// DTO for sending chatbot response
public class ChatResponsee
{
    public string Response { get; set; } = string.Empty;
}
