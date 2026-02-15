using Microsoft.AspNetCore.Mvc;
using MovieReviewSystem.Data;
using MovieReviewSystem.Models;
using MongoDB.Driver;

namespace MovieReviewSystem.Controllers
{
    public class AdminMovieController : Controller
    {
        private readonly MongoDbContext _context;

        public AdminMovieController(MongoDbContext context)
        {
            _context = context;
        }

       private bool IsAdmin()
{
    return HttpContext.Session.GetString("UserRole") == "Admin";
}

public IActionResult Dashboard()
{
    if (!IsAdmin())
        return Unauthorized();

    ViewBag.TotalUsers = _context.Users.CountDocuments(_ => true);
    ViewBag.TotalReviews = _context.Reviews.CountDocuments(_ => true);

    return View();
}


        public IActionResult Index()
        {
            if (!IsAdmin())
                return Unauthorized();

            var movies = _context.Movies.Find(_ => true).ToList();
            return View(movies);
        }

        public IActionResult Edit(string id)
        {
            if (!IsAdmin())
                return Unauthorized();

            var movie = _context.Movies.Find(m => m.Id == id).FirstOrDefault();
            return View(movie);
        }

        [HttpPost]
        public IActionResult Edit(Movie movie)
        {
            _context.Movies.ReplaceOne(m => m.Id == movie.Id, movie);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            if (!IsAdmin())
                return Unauthorized();

            _context.Movies.DeleteOne(m => m.Id == id);
            return RedirectToAction("Index");
        }
    }
}
