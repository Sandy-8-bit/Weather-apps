using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorWasmApp.Models
{
    public class FavoriteCity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; } = string.Empty; // Store user ID to associate favorites
        public string CityName { get; set; } = string.Empty;

        public bool IsHomeCity { get; set; } = false;
    }
}
