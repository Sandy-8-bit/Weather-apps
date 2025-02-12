using System.Net.Http.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigation;

    public event Action? OnAuthStateChanged;

    public AuthService(HttpClient httpClient, NavigationManager navigation)
    {
        _httpClient = httpClient;
        _navigation = navigation;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });

        if (response.IsSuccessStatusCode)
        {
            OnAuthStateChanged?.Invoke();  // Notify UI of login success
            return true;
        }

        return false;
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", new { Email = email, Password = password });

        if (response.IsSuccessStatusCode)
        {
            OnAuthStateChanged?.Invoke();
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("api/auth/logout", null);

        OnAuthStateChanged?.Invoke();  // Notify UI of logout
        _navigation.NavigateTo("/login", forceLoad: true);
    }

    public async Task<bool> RecoverPasswordAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/recover-password", new { Email = email });
        return response.IsSuccessStatusCode;
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<User>("api/auth/current-user");
        }
        catch
        {
            return null;
        }
    }

    public async Task<string?> GetUserEmailAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Email;
    }

    public async Task<bool> IsUserLoggedIn()
    {
        var user = await GetCurrentUserAsync();
        return user != null;
    }

    public class User
    {
        public string Email { get; set; } = string.Empty;
    }
}
