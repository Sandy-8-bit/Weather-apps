using BlazorWasmApp.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    [EnableCors("AllowBlazorWasm")]
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        // Get current weather by city name
        [HttpGet("{city}")]
        public async Task<ActionResult<WeatherResponse>> GetWeather(string city)
        {
            var weather = await _weatherService.GetWeatherAsync(city);
            if (weather == null)
                return NotFound($"Weather data not found for the city: {city}.");

            return Ok(weather);
        }

        // Get 5-day weather forecast by city name
        [HttpGet("{city}/forecast")]
        public async Task<ActionResult<ForecastResponse>> GetFiveDayForecast(string city)
        {
            var forecast = await _weatherService.GetFiveDayForecastAsync(city);
            if (forecast == null)
                return NotFound($"Weather forecast not found for the city: {city}.");

            return Ok(forecast);
        }

        // Add a city to favorites
        [HttpPost("favorites/{city}")]
        public ActionResult AddToFavorites(string city)
        {
            _weatherService.AddToFavorites(city);
            return Ok($"{city} has been added to favorites.");
        }

        // Remove a city from favorites
        [HttpDelete("favorites/{city}")]
        public ActionResult RemoveFromFavorites(string city)
        {
            _weatherService.RemoveFromFavorites(city);
            return Ok($"{city} has been removed from favorites.");
        }

        // Check if a city is in favorites
        [HttpGet("favorites/{city}")]
        public ActionResult<bool> IsCityInFavorites(string city)
        {
            var isInFavorites = _weatherService.IsCityInFavorites(city);
            return Ok(isInFavorites);
        }

        // Get weather for a city in favorites
        [HttpGet("favorites/{city}/weather")]
        public async Task<ActionResult<WeatherResponse>> GetWeatherFromFavoriteCity(string city)
        {
            var weather = await _weatherService.GetWeatherFromFavoriteCity(city);
            if (weather == null)
                return NotFound($"Weather data not found for the favorite city: {city}.");

            return Ok(weather);
        }

        // Get weather by coordinates
        [HttpGet("coordinates")]
        public async Task<ActionResult<WeatherResponse>> GetWeatherByCoordinates([FromQuery] double latitude, [FromQuery] double longitude)
        {
            var weather = await _weatherService.GetWeatherByCoordinatesAsync(latitude, longitude);
            if (weather == null)
                return NotFound("Weather data not found for the provided coordinates.");

            return Ok(weather);
        }

        // Clear weather cache
        [HttpDelete("cache")]
        public ActionResult ClearCache()
        {
            _weatherService.ClearCache();
            return Ok("Weather cache cleared.");
        }

        // New endpoint to send a rain notification email
        [HttpPost("send-rain-notification")]
        public async Task<ActionResult> SendRainNotificationEmail([FromBody] RainNotificationRequest request)
        {
            try
            {
                await _weatherService.SendRainNotificationEmail(request.UserEmail!, request.CityName!, request.Description!);
                return Ok("Rain notification email sent.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending email: {ex.Message}");
            }
        }
    }
}
