using Microsoft.AspNetCore.Mvc;
using MovieReviewSystem.Models;
using MovieReviewSystem.Data;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MovieReviewSystem.Controllers
{
    public class MovieController : Controller
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly MongoDbContext _db;

        public MovieController(MongoDbContext db)
        {
            _db = db;
        }

        // ================= SEARCH PAGE (GET) =================
        public async Task<IActionResult> Search()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            string apiKey = "ad292818";

            // Default popular movies
            ViewBag.Bollywood = await GetMovies("hindi", apiKey);
            ViewBag.Tollywood = await GetMovies("telugu", apiKey);
            ViewBag.Hollywood = await GetMovies("avengers", apiKey);

            return View();
        }

        // ================= SEARCH PAGE (POST) =================
        [HttpPost]
        public async Task<IActionResult> Search(string movieName, string language)
        {
            string apiKey = "ad292818";

            string searchTerm = movieName;
            if (!string.IsNullOrEmpty(language))
            {
                searchTerm = language; // Filter by language keyword
            }

            ViewBag.SearchResult = await GetMovies(searchTerm, apiKey);

            // Default sections (Optional)
            ViewBag.Bollywood = await GetMovies("hindi", apiKey);
            ViewBag.Tollywood = await GetMovies("telugu", apiKey);
            ViewBag.Hollywood = await GetMovies("avengers", apiKey);

            return View();
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(string imdbId)
        {
            string apiKey = "ad292818";
            string url = $"http://www.omdbapi.com/?apikey={apiKey}&i={imdbId}";

            var response = await _client.GetStringAsync(url);
            var movie = JsonConvert.DeserializeObject<OmdbMovieViewModel>(response);

            var reviews = _db.Reviews.Find(r => r.MovieId == imdbId).ToList();

            ViewBag.AvgRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
            ViewBag.UserCount = reviews.Count;

            return View(movie);
        }

        // ================= ADD / UPDATE REVIEW =================
        [HttpPost]
        public IActionResult AddReview(string movieId, int rating, string comment)
        {
            var userId = HttpContext.Session.GetString("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var existing = _db.Reviews
                .Find(r => r.MovieId == movieId && r.UserID == userId)
                .FirstOrDefault();

            if (existing != null)
            {
                existing.Rating = rating;
                existing.Comment = comment;
                existing.CreatedAt = DateTime.Now;

                _db.Reviews.ReplaceOne(r => r.Id == existing.Id, existing);
            }
            else
            {
                var review = new Review
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    MovieId = movieId,
                    UserID = userId,
                    UserName = HttpContext.Session.GetString("UserName"),
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.Now
                };

                _db.Reviews.InsertOne(review);
            }

            return RedirectToAction("Details", new { imdbId = movieId });
        }

        // ================= TRENDING =================
        public async Task<IActionResult> Trending()
        {
            string apiKey = "ad292818";
            return View(await GetMovies("2025", apiKey));
        }

        // ================= HELPER =================
        private async Task<List<OmdbMovieViewModel>> GetMovies(string term, string key)
        {
            string url = $"http://www.omdbapi.com/?apikey={key}&s={term}";
            var res = await _client.GetStringAsync(url);
            var json = JObject.Parse(res);

            return json["Search"]?.ToObject<List<OmdbMovieViewModel>>() ?? new List<OmdbMovieViewModel>();
        }
    }
}

