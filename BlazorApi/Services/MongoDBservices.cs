using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Supabase.Gotrue;

public class MongoDBService
{
    private readonly IMongoCollection<FavoriteCity> _favoriteCities;

    public MongoDBService(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
        var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
        _favoriteCities = database.GetCollection<FavoriteCity>(configuration["MongoDB:CollectionName"]);
    }

    // Add a city to the favorites list
    public async Task AddFavoriteCityAsync(string userId, string cityName)
    {
        try
        {
            var existing = await _favoriteCities
                .Find(x => x.UserId == userId && x.CityName == cityName)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await _favoriteCities.InsertOneAsync(new FavoriteCity { UserId = userId, CityName = cityName });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding favorite city: {ex.Message}");
        }
    }

    // Retrieve favorite cities for a specific user
    public async Task<List<FavoriteCity>> GetFavoriteCitiesAsync(string userId)
    {
        try
        {
            return await _favoriteCities.Find(city => city.UserId == userId).ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching favorite cities: {ex.Message}");
            return new List<FavoriteCity>();
        }
    }

    // Remove a city from the favorites list
    public async Task RemoveFavoriteCityAsync(string userId, string cityName)
    {
        try
        {
            await _favoriteCities.DeleteOneAsync(city => city.UserId == userId && city.CityName == cityName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing favorite city: {ex.Message}");
        }
    }

    // Set a city as the home city
    public async Task SetHomeCityAsync(string userId, string cityName)
    {
        try
        {
            // Remove the current home city
            var removeHomeCityFilter = Builders<FavoriteCity>.Filter.Eq(x => x.UserId, userId) &
                                       Builders<FavoriteCity>.Filter.Eq(x => x.IsHomeCity, true);
            var unsetHomeCityUpdate = Builders<FavoriteCity>.Update.Set(x => x.IsHomeCity, false);
            await _favoriteCities.UpdateOneAsync(removeHomeCityFilter, unsetHomeCityUpdate);

            // Set the new home city
            var setHomeCityFilter = Builders<FavoriteCity>.Filter.Eq(x => x.UserId, userId) &
                                    Builders<FavoriteCity>.Filter.Eq(x => x.CityName, cityName);
            var setHomeCityUpdate = Builders<FavoriteCity>.Update.Set(x => x.IsHomeCity, true);
            await _favoriteCities.UpdateOneAsync(setHomeCityFilter, setHomeCityUpdate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting home city: {ex.Message}");
        }
    }

    // Remove the home city for the user
    public async Task RemoveHomeCityAsync(string userId)
    {
        try
        {
            var filter = Builders<FavoriteCity>.Filter.And(
                Builders<FavoriteCity>.Filter.Eq(x => x.UserId, userId),
                Builders<FavoriteCity>.Filter.Eq(x => x.IsHomeCity, true)
            );

            var update = Builders<FavoriteCity>.Update.Set(x => x.IsHomeCity, false);

            await _favoriteCities.UpdateOneAsync(filter, update);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing home city: {ex.Message}");
        }
    }


    // Get the home city for a specific user
    public async Task<FavoriteCity?> GetHomeCityAsync(string userId)
    {
        try
        {
            return await _favoriteCities
                .Find(city => city.UserId == userId && city.IsHomeCity == true)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching home city: {ex.Message}");
            return null;
        }
    }

    // Check if a city is in the user's favorite list
    public async Task<bool> isFavouritecityAsync(string userId, string cityName)
    {
        try
        {
            var filter = Builders<FavoriteCity>.Filter.And(
                Builders<FavoriteCity>.Filter.Eq(f => f.UserId, userId),
                Builders<FavoriteCity>.Filter.Eq(f => f.CityName, cityName)
            );

            var favoriteCity = await _favoriteCities.Find(filter).FirstOrDefaultAsync();
            return favoriteCity != null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking favorite city: {ex.Message}");
            return false;
        }
    }
}

public class FavoriteCity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("UserId")]
    public string? UserId { get; set; }

    [BsonElement("CityName")]
    public string? CityName { get; set; }

    [BsonElement("IsHomeCity")]
    public bool IsHomeCity { get; set; }

   
}
