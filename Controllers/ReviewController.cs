using Microsoft.AspNetCore.Mvc;
using MovieReviewSystem.Data;
using MovieReviewSystem.Models;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using MovieReviewSystem.Models.ViewModels;



namespace MovieReviewSystem.Controllers
{
    public class ReviewController : Controller
    {
        private readonly MongoDbContext _context;

        public ReviewController(MongoDbContext context)
        {
            _context = context;
        }

        // ✅ ADD REVIEW
     
       [HttpPost]
public IActionResult AddReview(string movieId, int rating, string comment)
{
    var userId = HttpContext.Session.GetString("UserID");
    if (userId == null)
        return RedirectToAction("Login", "Account");

    var existingReview = _context.Reviews.Find(r => r.MovieId == movieId && r.UserID == userId)
                                         .FirstOrDefault();

    if (existingReview != null)
    {
        existingReview.Rating = rating;
        existingReview.Comment = comment;
        existingReview.CreatedAt = DateTime.Now;
        _context.Reviews.ReplaceOne(r => r.Id == existingReview.Id, existingReview);

        TempData["SuccessMessage"] = "Review updated successfully!";
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

        _context.Reviews.InsertOne(review);

        TempData["SuccessMessage"] = "Review added successfully!";
    }

    return RedirectToAction("MyReviews");
}


        //  MY REVIEWS
   public async Task<IActionResult> MyReviews()
{
    var userId = HttpContext.Session.GetString("UserID");
    if (userId == null)
        return RedirectToAction("Login", "Account");

    var reviews = _context.Reviews
        .Find(r => r.UserID == userId)
        .ToList();

    // ---------- AI LOGIC ----------
    var genreCount = new Dictionary<string, int>();

    foreach (var r in reviews.Where(x => x.Rating >= 4))
    {
        var json = await new HttpClient()
            .GetStringAsync($"http://www.omdbapi.com/?apikey=ad292818&i={r.MovieId}");

        dynamic movie = JsonConvert.DeserializeObject(json);

        if (movie?.Genre != null)
        {
            foreach (var g in movie.Genre.ToString().Split(','))
            {
                var genre = g.Trim();
                if (!genreCount.ContainsKey(genre))
                    genreCount[genre] = 0;

                genreCount[genre]++;
            }
        }
    }

    var favGenre = genreCount.Any()
        ? genreCount.OrderByDescending(x => x.Value).First().Key
        : "Drama";

    // ---------- GRAPH ----------
    var ratingGraph = reviews
        .GroupBy(r => r.Rating)
        .Select(g => new RatingGraphVM
        {
            Rating = g.Key,
            Count = g.Count()
        })
        .OrderBy(x => x.Rating)
        .ToList();

    // ---------- REVIEW LIST ----------
    var reviewList = new List<MyReviewVM>();
    foreach (var r in reviews)
    {
        var json = await new HttpClient()
            .GetStringAsync($"http://www.omdbapi.com/?apikey=ad292818&i={r.MovieId}");
        dynamic movie = JsonConvert.DeserializeObject(json);

        reviewList.Add(new MyReviewVM
        {
            Review = r,
            Poster = movie?.Poster,
            Title = movie?.Title
        });
    }

    var vm = new MyReviewsPageVM
    {
        Reviews = reviewList,
        RatingGraph = ratingGraph,
        FavGenre = favGenre,
        Avg = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
        Count = reviews.Count
    };

    return View(vm);
}


        public IActionResult Delete(string id)
        {
            _context.Reviews.DeleteOne(r => r.Id == id);
            return RedirectToAction("MyReviews");
        }

        public IActionResult Edit(string id)
        {
            var review = _context.Reviews.Find(r => r.Id == id).FirstOrDefault();
            return View(review);
        }

        [HttpPost]
     [HttpPost]
[HttpPost]
public IActionResult Edit(Review r)
{
    var old = _context.Reviews.Find(x => x.Id == r.Id).FirstOrDefault();
    if (old == null) return NotFound();

    old.Rating = r.Rating;
    old.Comment = r.Comment;
    old.CreatedAt = DateTime.Now;

    _context.Reviews.ReplaceOne(x => x.Id == r.Id, old);

    // ✅ Set TempData for notification
    TempData["SuccessMessage"] = "Review updated successfully!";

    return RedirectToAction("MyReviews");
}

    }
}
