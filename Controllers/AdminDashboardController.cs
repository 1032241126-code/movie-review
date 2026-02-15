using Microsoft.AspNetCore.Mvc;
using MovieReviewSystem.Data;
using MovieReviewSystem.Models;
using MongoDB.Driver;

namespace MovieReviewSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly MongoDbContext _context;

        public AdminController(MongoDbContext context)
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
    }
}

