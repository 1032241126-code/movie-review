using MongoDB.Driver;
using MovieReviewSystem.Models;

namespace MovieReviewSystem.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            _database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        }

        public IMongoCollection<User> Users =>
            _database.GetCollection<User>("Users");

        public IMongoCollection<Review> Reviews =>
            _database.GetCollection<Review>("Reviews");

             public IMongoCollection<Movie> Movies =>          // 🔥 THIS LINE
            _database.GetCollection<Movie>("Movies");
    }
}
