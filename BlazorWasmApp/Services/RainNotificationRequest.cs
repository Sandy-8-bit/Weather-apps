namespace BlazorWasmApp.Services
{
    internal class RainNotificationRequest
    {
        public string? UserEmail { get; set; }
        public string? CityName { get; set; }
        public string? Description { get; set; }
    }
}