using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherApp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SupabaseAuthService>();
builder.Services.AddSingleton<MongoDBService>();
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddSingleton<GeminiChatService>();
builder.Services.AddSingleton<WeatherChatService>();
builder.Services.AddHttpContextAccessor(); // 👈 This is required for IHttpContextAccessor
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm",
        policy => policy.WithOrigins("http://localhost:8080") // Your Blazor WASM URL
                        .AllowAnyMethod()
                        .AllowAnyHeader()
     
                        .AllowCredentials());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy globally or for specific controllers
app.UseCors("AllowBlazorWasm");

app.UseAuthorization();
app.MapControllers(); // If using API controllers
// app.MapBlazorHub(); // Uncomment this if using Blazor Server

app.Run();
