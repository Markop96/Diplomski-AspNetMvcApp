using M_RENT_Aplikacija.Data;
using M_RENT_Aplikacija.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(int rating, string comment)
    {
        var recenzija = new Recenzija
        {
            Rating = rating,
            Comment = comment,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        _context.Recenzije.Add(recenzija);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }



    public IActionResult Index(string sortOrder)
    {
        ViewData["RatingSortParm"] = String.IsNullOrEmpty(sortOrder) ? "rating_desc" : "";

        var recenzije = from r in _context.Recenzije.Include(r => r.User)
                        select r;

        switch (sortOrder)
        {
            case "rating_desc":
                recenzije = recenzije.OrderByDescending(r => r.Rating);
                break;
            default:
                recenzije = recenzije.OrderBy(r => r.Rating);
                break;
        }

        return View(recenzije.ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var recenzija = _context.Recenzije.Find(id);
        if (recenzija != null)
        {
            _context.Recenzije.Remove(recenzija);
            _context.SaveChanges();
        }
        return RedirectToAction(nameof(Index));
    }
}
