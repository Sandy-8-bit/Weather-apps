using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WeatherApp.Services;
using BlazorWasmApp.Models;

[EnableCors("AllowBlazorWasm")]
[Route("api/weatherchat")]
[ApiController]
public class WeatherChatController : ControllerBase
{
    private readonly WeatherChatService _weatherChatService;

    public WeatherChatController(WeatherChatService weatherChatService)
    {
        _weatherChatService = weatherChatService ?? throw new ArgumentNullException(nameof(weatherChatService));
    }

    [HttpPost]
    public async Task<IActionResult> GetChatbotResponse([FromBody] ChatRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { message = "Message cannot be empty." });
        }

        try
        {
            var (chatResponse, weather) = await _weatherChatService.ProcessMessageAsync(request.Message);

            return Ok(new ChatResponse
            {
                Response = chatResponse,
                Weather = weather
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in WeatherChatController: {ex.Message}");
            return StatusCode(500, new { message = "An error occurred while processing the request." });
        }
    }
}

// DTO for receiving user message
public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}

// DTO for sending chatbot response
public class ChatResponse
{
    public string Response { get; set; } = string.Empty;
    public WeatherResponse? Weather { get; set; }
}