using System.Net.Http.Json;
using BlazorWasmApp.Models;

namespace BlazorWasmApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            return await _httpClient.GetFromJsonAsync<WeatherResponse>($"api/weather/{city}");
        }

        public async Task<ForecastResponse?> GetFiveDayForecastAsync(string city)
        {
            return await _httpClient.GetFromJsonAsync<ForecastResponse>($"api/weather/{city}/forecast");
        }

        public async Task AddToFavoritesAsync(string city)
        {
            await _httpClient.PostAsync($"api/weather/favorites/{city}", null);
        }

        public async Task RemoveFromFavoritesAsync(string city)
        {
            await _httpClient.DeleteAsync($"api/weather/favorites/{city}");
        }

        public async Task<bool> IsCityInFavoritesAsync(string city)
        {
            return await _httpClient.GetFromJsonAsync<bool>($"api/weather/favorites/{city}");
        }

        public async Task SendRainNotificationEmailAsync(string email, string city, string description)
        {
            var request = new RainNotificationRequest { UserEmail = email, CityName = city, Description = description };
            await _httpClient.PostAsJsonAsync("api/weather/send-rain-notification", request);
        }
    }
}
