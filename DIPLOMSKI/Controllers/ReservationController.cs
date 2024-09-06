using M_RENT_Aplikacija.Data;
using M_RENT_Aplikacija.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly StripeService _stripeService;

    public ReservationsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, StripeService stripeService)
    {
        _context = context;
        _userManager = userManager;
        _stripeService = stripeService;
    }

    public async Task<IActionResult> Index()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .ToListAsync();
        return View(reservations);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Create()
    {
        // Filtrirajte automobila koji su aktivni
        var cars = await _context.Automobili
            .Where(a => a.IsActive) // Dodajte uslov za aktivne automobile
            .ToListAsync();

        // Kreirajte SelectList sa filtriranim automobilima
        ViewData["Cars"] = new SelectList(cars, "Id", "Name");

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DateTime dateStart, DateTime dateEnd, int idCar)
    {
        if (dateStart > dateEnd)
        {
            ModelState.AddModelError("", "Start date cannot be later than end date.");
            var cars = await _context.Automobili.ToListAsync();
            ViewData["Cars"] = new SelectList(cars, "Id", "Name");
            return View();
        }

        var userId = _userManager.GetUserId(User);
        var car = await _context.Automobili.FindAsync(idCar);

        if (car == null)
        {
            ModelState.AddModelError("", "Selected car is not available.");
            var cars = await _context.Automobili.ToListAsync();
            ViewData["Cars"] = new SelectList(cars, "Id", "Name");
            return View();
        }

        var numberOfDays = (dateEnd - dateStart).Days + 1;
        var fullPrice = numberOfDays * car.Price;

        var reservation = new Reservation
        {
            DateStart = dateStart,
            DateEnd = dateEnd,
            IdCar = idCar,
            IdUser = userId,
            NumberOfDays = numberOfDays,
            FullPrice = fullPrice,
            IsPaid = false // Set to false when creating
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("Zaposleni")))
        {
            return RedirectToAction(nameof(Index));
        }
        else
        {
            return RedirectToAction(nameof(MyReservations));
        }
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Car)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (reservation == null)
        {
            return NotFound();
        }

        // Check if there are any payments for this reservation
        var hasPayments = await _context.Payments
            .AnyAsync(p => p.ReservationId == id);

        ViewData["HasPayments"] = hasPayments;

        var cars = await _context.Automobili.ToListAsync();
        ViewData["Cars"] = new SelectList(cars, "Id", "Name", reservation.IdCar);

        return View(reservation);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DateTime dateStart, DateTime dateEnd, int idCar, bool isPaid)
    {
        if (id != id)
        {
            return NotFound();
        }

        if (dateStart > dateEnd)
        {
            ModelState.AddModelError("", "Start date cannot be later than end date.");
            var cars = await _context.Automobili.ToListAsync();
            ViewData["Cars"] = new SelectList(cars, "Id", "Name");
            return View();
        }

        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null)
        {
            return NotFound();
        }

        var car = await _context.Automobili.FindAsync(idCar);
        if (car == null)
        {
            ModelState.AddModelError("", "Selected car is not available.");
            var cars = await _context.Automobili.ToListAsync();
            ViewData["Cars"] = new SelectList(cars, "Id", "Name");
            return View(reservation);
        }

        reservation.DateStart = dateStart;
        reservation.DateEnd = dateEnd;
        reservation.IdCar = idCar;
        reservation.NumberOfDays = (dateEnd - dateStart).Days + 1;
        reservation.FullPrice = reservation.NumberOfDays * car.Price;
        reservation.IsPaid = isPaid; // Update payment status

        try
        {
            _context.Update(reservation);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReservationExists(reservation.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ReservationExists(int id)
    {
        return _context.Reservations.Any(e => e.Id == id);
    }

    [HttpGet]
    public JsonResult GetPriceById(int id)
    {
        var car = _context.Automobili.FirstOrDefault(c => c.Id == id);
        if (car == null)
        {
            return Json(0);
        }
        return Json(car.Price);
    }

    [HttpGet]
    public async Task<IActionResult> Search(string searchQuery)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            return RedirectToAction(nameof(Index));
        }

        var reservations = await _context.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .Where(r => r.User.UserName.Contains(searchQuery))
            .ToListAsync();

        return View("Index", reservations);
    }

    public async Task<IActionResult> MyReservations()
    {
        var userId = _userManager.GetUserId(User);
        var reservations = await _context.Reservations
            .Include(r => r.Car)
            .Include(r => r.User)
            .Where(r => r.IdUser == userId)
            .ToListAsync();
        return View(reservations);
    }





    [HttpGet]
    public async Task<IActionResult> Pay(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        if (reservation.IsPaid)
        {
            return RedirectToAction(nameof(Index)); // Redirect if already paid
        }

        ViewBag.PublishableKey = _stripeService.GetPublishableKey();

        return View(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Pay(int id, string stripeToken)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Car)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (reservation == null)
        {
            return NotFound();
        }

        if (reservation.IsPaid)
        {
            return RedirectToAction(nameof(Index)); 
        }

        var charge = await _stripeService.CreateCharge(reservation.FullPrice, "usd", stripeToken, $"Reservation {reservation.Id}");

        if (charge.Status == "succeeded")
        {
            reservation.IsPaid = true;
            _context.Update(reservation);

            var payment = new Payment
            {
                UserId = reservation.IdUser,
                ReservationId = reservation.Id,
                Amount = reservation.FullPrice,
                PaymentDate = DateTime.UtcNow,
                StripeChargeId = charge.Id
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyReservations));
        }

        ModelState.AddModelError("", "Payment failed. Please try again.");
        ViewBag.PublishableKey = _stripeService.GetPublishableKey();
        return View(reservation);
    }
}
