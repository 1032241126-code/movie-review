using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MovieReviewSystem.Models
{
  
    public class Movie
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

     
   
}

 public class Movie
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Poster { get; set; }
        public string Year { get; set; }
        public string ImdbId { get; set; }
    }




