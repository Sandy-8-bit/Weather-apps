using System.Net.Http.Json;

using BlazorWasmApp.Models;
public class FavoriteService
{
    private readonly HttpClient _http;

    public FavoriteService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<FavoriteCity>> GetFavoritesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<FavoriteCity>>("api/favorites") ?? new();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching favorites: {ex.Message}");
            return new();
        }
    }

    public async Task<bool> AddFavoriteCityAsync(string city)
    {
        var response = await _http.PostAsync($"api/favorites/{city}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveFavoriteCityAsync(string city)
    {
        var response = await _http.DeleteAsync($"api/favorites/{city}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetHomeCityAsync(string city)
    {
        var response = await _http.PostAsync($"api/favorites/{city}/home", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<FavoriteCity?> GetHomeCityAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<FavoriteCity>("api/favorites/home");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching home city: {ex.Message}");
            return null;
        }
    }
}
