using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp.Services
{
    public class GeminiChatService
    {
        private readonly HttpClient _httpClient;
        private readonly string _geminiApiKey;

        public GeminiChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _geminiApiKey = configuration["Gemini:ApiKey"] ?? throw new ArgumentNullException("GeminiApiKey");
        }

        private class GeminiRequest
        {
            public List<ChatMessage> contents { get; set; } = new();
        }

        private class ChatMessage
        {
            public string role { get; set; } = string.Empty;
            public List<ContentPart> parts { get; set; } = new();
        }

        private class ContentPart
        {
            public string text { get; set; } = string.Empty;
        }

        public async Task<string> GetGeminiResponseAsync(string userMessage)
        {
            try
            {
                var systemPrompt = "You are a helpful AI assistant. Answer user queries in a clear, friendly, and conversational manner.";

                var messages = new List<ChatMessage>
                {
                    new ChatMessage { role = "user", parts = new List<ContentPart> { new ContentPart { text = systemPrompt } } },
                    new ChatMessage { role = "user", parts = new List<ContentPart> { new ContentPart { text = userMessage } } }
                };

                var request = new GeminiRequest { contents = messages };

                var response = await _httpClient.PostAsJsonAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_geminiApiKey}",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Gemini API error: {response.StatusCode}");
                    return "Sorry, I couldn't process your request at the moment.";
                }

                var content = await response.Content.ReadFromJsonAsync<JsonDocument>();

                if (content == null || !content.RootElement.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
                {
                    return "Sorry, I couldn't generate a response.";
                }

                return candidates[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "Sorry, no response available.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting Gemini response: {ex.Message}");
                return "I'm having trouble processing your request. Please try again later.";
            }
        }
    }
}
