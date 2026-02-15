using Microsoft.AspNetCore.Mvc;
using MovieReviewSystem.Data;
using MongoDB.Driver;

namespace MovieReviewSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MongoDbContext _context;

        public DashboardController(MongoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserID") == null)
                return RedirectToAction("Login", "Account");

            var userId = HttpContext.Session.GetString("UserID");

            ViewBag.TotalReviews = _context.Reviews.CountDocuments(r => r.UserID == userId);
            ViewBag.AvgRating = _context.Reviews
                .Find(r => r.UserID== userId)
                .ToList()
                .DefaultIfEmpty()
                .Average(r => r?.Rating ?? 0);

            return View();
        }
    }
}


