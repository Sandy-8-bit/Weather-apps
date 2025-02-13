using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorWasmApp;
using MudBlazor.Services;
using Supabase;
using System.Net.Http;
using BlazorWasmApp.Services;
using Blazored.LocalStorage;
using Supabase.Gotrue;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Adding configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add HttpClient with the backend API base URL
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:8081"), // API base URL
    DefaultRequestHeaders = { { "Accept", "application/json" } }
});

builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FavoriteService>();
builder.Services.AddMudServices();
builder.Services.AddScoped<ToasterService>();
builder.Services.AddBlazoredLocalStorage(); 

// Configure Supabase Authentication
builder.Services.AddScoped(sp =>
{
    var supabaseUrl = builder.Configuration["SupabaseUrl"] ?? "https://zmbwovmoriuapqluaxjn.supabase.co";
    var supabaseKey = builder.Configuration["SupabaseKey"] ?? "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InptYndvdm1vcml1YXBxbHVheGpuIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzczNTExNzEsImV4cCI6MjA1MjkyNzE3MX0.1FeFwemTq-y6GPRMhN-DJf2DQ2-1qRtKA9qrShVDOEU";

    var supabaseOptions = new SupabaseOptions
    {
        AutoRefreshToken = true, // Auto-refreshes the token when it expires
        AutoConnectRealtime = true // Enables real-time connections
    };

    var client = new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions);
    return client;
});

// Build and run the Blazor WebAssembly app
await builder.Build().RunAsync();
