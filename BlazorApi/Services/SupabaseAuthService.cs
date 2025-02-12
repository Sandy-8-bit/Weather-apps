using Supabase.Gotrue;
using System;
using System.Threading.Tasks;

public class SupabaseAuthService
{
    private readonly Supabase.Client _client;

    public SupabaseAuthService()
    {
        var options = new Supabase.ClientOptions
        {
            AutoRefreshToken = true,
            PersistSession = false // No session persistence in backend
        };

        _client = new Supabase.Client(
            "https://zmbwovmoriuapqluaxjn.supabase.co", // Replace with your actual Supabase URL
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InptYndvdm1vcml1YXBxbHVheGpuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzczNTExNzEsImV4cCI6MjA1MjkyNzE3MX0.1FeFwemTq-y6GPRMhN-DJf2DQ2-1qRtKA9qrShVDOEU", // Replace with your actual API key
            options
        );

        _client.InitializeAsync().Wait(); // Initialize synchronously
    }

    // ? Login user - Returns session
    public async Task<Session?> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignIn(email, password);
            return session;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
            throw;
        }
    }

    // ? Register user - Returns session
    public async Task<Session?> RegisterAsync(string email, string password)
    {
        try
        {
            var session = await _client.Auth.SignUp(email, password);
            return session;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration failed: {ex.Message}");
            throw;
        }
    }

    // ? Logout user
    public async Task LogoutAsync()
    {
        await _client.Auth.SignOut();
    }

    // ? Get the current session - FIXED
    public Task<Session?> GetSessionAsync()
    {
        return Task.FromResult(_client.Auth.CurrentSession);
    }

    // ? Get the current authenticated user - FIXED
    public Task<User?> GetCurrentUserAsync()
    {
        try
        {
            var session = _client.Auth.CurrentSession; // ? FIXED: Using `CurrentSession`
            return Task.FromResult(session?.User); // Returns the user if a session exists
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get current user: {ex.Message}");
            return Task.FromResult<User?>(null);
        }
    }

    // ? Send password recovery email
    public async Task SendPasswordRecoveryEmailAsync(string email)
    {
        try
        {
            await _client.Auth.ResetPasswordForEmail(email);
            Console.WriteLine($"Password recovery email sent to: {email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Password recovery failed: {ex.Message}");
            throw;
        }
    }
}
