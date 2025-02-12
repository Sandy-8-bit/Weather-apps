using BlazorWasmApp.Models;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "0396fe1fc3e16a3fcca570677d66ae8d"; // Your OpenWeather API key
        private readonly IConfiguration _configuration;
        private readonly SupabaseAuthService _authService;
        private readonly Dictionary<string, WeatherResponse> _weatherCache = new();

        public List<string> FavoriteCities { get; private set; } = new();

        private const string CurrentWeatherEndpoint = "https://api.openweathermap.org/data/2.5/weather";
        private const string ForecastEndpoint = "https://api.openweathermap.org/data/2.5/forecast";

        public WeatherService(HttpClient httpClient, IConfiguration configuration, SupabaseAuthService authService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _authService = authService;
        }

        // Fetch current weather data by city
        public async Task<WeatherResponse?> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("City name cannot be empty.");
                return null;
            }

            if (_weatherCache.TryGetValue(city, out var cachedWeather))
            {
                Console.WriteLine($"Returning cached weather data for: {city}");
                return cachedWeather;
            }

            try
            {
                var response = await _httpClient.GetAsync($"{CurrentWeatherEndpoint}?q={city}&appid={_apiKey}&units=metric");

                if (response.IsSuccessStatusCode)
                {
                    var weather = await response.Content.ReadFromJsonAsync<WeatherResponse>();
                    if (weather != null)
                    {
                        _weatherCache[city] = weather;
                        await HandleRainNotification(weather, city); // Handle rain notification via backend API
                        return weather;
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch weather data for {city}. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather for {city}: {ex.Message}");
            }

            return null;
        }

        private async Task HandleRainNotification(WeatherResponse weather, string city)
        {
            if (weather.weather.Any(w => w.main.Equals("Rain", StringComparison.OrdinalIgnoreCase)))
            {
                var user = await _authService.GetCurrentUserAsync(); // ? Await the async method
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    var rainNotificationRequest = new RainNotificationRequest
                    {
                        UserEmail = user.Email!, // ? Now it's correctly awaited
                        CityName = city,
                        Description = weather.weather.FirstOrDefault()?.description ?? "No description available"
                    };

                    // Call backend API to send the rain notification email
                    var response = await _httpClient.PostAsJsonAsync("api/weather/send-rain-notification", rainNotificationRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Rain notification email sent successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to send rain notification email. Status: {response.StatusCode}");
                    }
                }

            }
        }

        // Fetch 5-day weather forecast for a city
        public async Task<ForecastResponse?> GetFiveDayForecastAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{ForecastEndpoint}?q={city}&appid={_apiKey}&units=metric");

                if (response.IsSuccessStatusCode)
                {
                    var forecastResponse = await response.Content.ReadFromJsonAsync<ForecastResponse>();
                    if (forecastResponse != null)
                    {
                        // Optionally filter the list to include only data at 12:00 PM
                        forecastResponse.list = forecastResponse.list
                            .Where(f =>
                            {
                                if (DateTime.TryParse(f.dt_txt, out var forecastDate))
                                {
                                    return forecastDate.Hour == 12;
                                }
                                return false;
                            })
                            .ToList();

                        return forecastResponse;
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch forecast for {city}. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching forecast for {city}: {ex.Message}");
            }

            return null;
        }

        // Manage favorite cities
        public void AddToFavorites(string city)
        {
            if (!FavoriteCities.Contains(city, StringComparer.OrdinalIgnoreCase))
            {
                FavoriteCities.Add(city);
                Console.WriteLine($"Added {city} to favorites.");
            }
        }

        public void RemoveFromFavorites(string city)
        {
            if (FavoriteCities.Remove(city))
            {
                Console.WriteLine($"Removed {city} from favorites.");
            }
        }

        public bool IsCityInFavorites(string city) =>
            FavoriteCities.Contains(city, StringComparer.OrdinalIgnoreCase);

        public async Task<WeatherResponse?> GetWeatherFromFavoriteCity(string city)
        {
            if (!IsCityInFavorites(city))
            {
                Console.WriteLine($"City {city} is not in the favorites list.");
                return null;
            }

            return await GetWeatherAsync(city);
        }

        // Get weather by coordinates
        public async Task<WeatherResponse?> GetWeatherByCoordinatesAsync(double latitude, double longitude)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{CurrentWeatherEndpoint}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<WeatherResponse>();
                }
                else
                {
                    Console.WriteLine($"Failed to fetch weather by coordinates. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching weather by coordinates: {ex.Message}");
            }

            return null;
        }

        public void ClearCache()
        {
            _weatherCache.Clear();
            Console.WriteLine("Cache cleared.");
        }

        // Send Rain Notification Email (Backend will call this)
        public async Task SendRainNotificationEmail(string userEmail, string cityName, string description)
        {
            try
            {
                var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
                {
                    Port = int.Parse(_configuration["EmailSettings:Port"]!),
                    Credentials = new NetworkCredential(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]
                    ),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:FromAddress"]!),
                    Subject = $"Rain Alert for {cityName}",
                    Body = $"Hello, there is rain in {cityName}. Description: {description ?? "No description available"}. Please take necessary precautions.",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(userEmail);
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending rain notification email: {ex.Message}");
            }
        }
    }

    // Request model for sending rain notification
    public class RainNotificationRequest
    {
        public string? UserEmail { get; set; }
        public string? CityName { get; set; }
        public string? Description { get; set; }
    }
}
