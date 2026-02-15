using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieReviewSystem.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string MovieId { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}




