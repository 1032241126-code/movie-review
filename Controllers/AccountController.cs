using Microsoft.AspNetCore.Mvc;
using MovieReviewSystem.Data;
using MovieReviewSystem.Models;
using MongoDB.Driver;

public class AccountController : Controller
{
    private readonly MongoDbContext _context;

    public AccountController(MongoDbContext context)
    {
        _context = context;
    }

    // ================= REGISTER GET =================
    public IActionResult Register()
    {
        if (HttpContext.Session.GetString("UserID") != null)
            return RedirectToAction("Search", "Movie");

        return View();
    }

    // ================= REGISTER POST =================
    [HttpPost]
    public IActionResult Register(User user)
    {
        var existing = _context.Users
            .Find(u => u.Email == user.Email)
            .FirstOrDefault();

        if (existing != null)
        {
            ViewBag.Error = "Email already registered.";
            return View();
        }

        _context.Users.InsertOne(user);

        // ✅ SESSION (single standard key)
        HttpContext.Session.SetString("UserID", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);

        return RedirectToAction("Search", "Movie");
    }

    // ================= LOGIN GET =================
    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("UserID") != null)
            return RedirectToAction("Search", "Movie");

        return View();
    }

    // ================= LOGIN POST =================
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        var user = _context.Users
            .Find(u => u.Email == email && u.Password == password)
            .FirstOrDefault();

        if (user == null)
        {
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        // ✅ SESSION
        HttpContext.Session.SetString("UserID", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);
HttpContext.Session.SetString("UserRole", user.Role);

        return RedirectToAction("Search", "Movie");
    }

    // ================= LOGOUT =================
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
