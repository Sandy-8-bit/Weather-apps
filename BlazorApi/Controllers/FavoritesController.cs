using Microsoft.AspNetCore.Mvc;
using WeatherApp.Services;
using BlazorWasmApp.Models;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApp.Controllers
{
    [EnableCors("AllowBlazorWasm")]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly SupabaseAuthService _authService;

        public FavoritesController(MongoDBService mongoDBService, SupabaseAuthService authService)
        {
            _mongoDBService = mongoDBService;
            _authService = authService;
        }

        // ? Add a city to favorites
        [HttpPost("{city}")]
        public async Task<ActionResult> AddFavoriteCityAsync(string city)
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            try
            {
                var isAlreadyFavorite = await _mongoDBService.isFavouritecityAsync(userId!, city);
                if (isAlreadyFavorite)
                {
                    return Conflict($"{city} is already in your favorites.");
                }
                await _mongoDBService.AddFavoriteCityAsync(userId!, city);
                return Ok($"{city} added to favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ? Get all favorite cities
        [HttpGet]
        public async Task<ActionResult<List<FavoriteCity>>> GetFavorites()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                var favorites = await _mongoDBService.GetFavoriteCitiesAsync(userId!);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ? Remove a city from favorites
        [HttpDelete("{city}")]
        public async Task<ActionResult> RemoveFavoriteCity(string city)
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                await _mongoDBService.RemoveFavoriteCityAsync(userId!, city);
                return Ok($"{city} removed from favorites.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ? Set a city as the home city
        [HttpPost("{city}/home")]
        public async Task<ActionResult> SetHomeCity(string city)
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            if (string.IsNullOrEmpty(city))
            {
                return BadRequest("City name cannot be empty.");
            }

            try
            {
                await _mongoDBService.SetHomeCityAsync(userId!, city);
                return Ok($"{city} is now your home city.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ? Get the current home city
        [HttpGet("home")]
        public async Task<ActionResult<FavoriteCity>> GetHomeCity()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                var homeCity = await _mongoDBService.GetHomeCityAsync(userId!);
                if (homeCity == null)
                {
                    return NotFound("No home city set.");
                }

                return Ok(homeCity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // ? Remove the current home city
        [HttpDelete("home")]
        public async Task<ActionResult> RemoveHomeCity()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return Unauthorized("User not authenticated.");
            }

            var userId = currentUser.Id;
            try
            {
                await _mongoDBService.RemoveHomeCityAsync(userId!);
                return Ok("Home city has been removed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
