using Microsoft.AspNetCore.Mvc;
using M_RENT_Aplikacija.Data;
using M_RENT_Aplikacija.Models;
using System.Linq;
using M_RENT_Aplikacija.Models.ViewModels;

namespace M_RENT_Aplikacija.Controllers
{
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CarController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            var automobili = _applicationDbContext.Automobili.ToList();
            return View(automobili);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Add")]
        public IActionResult SubmitCar(Automobil addAutomobil)
        {
            var automobil = new Automobil()
            {
                Name = addAutomobil.Name,
                Price = addAutomobil.Price,
                Id = addAutomobil.Id,
                IsActive = true
            };

            _applicationDbContext.Automobili.Add(automobil);
            _applicationDbContext.SaveChanges();

            return View("Add");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var automobil = _applicationDbContext.Automobili.Find(id);
            if (automobil == null)
            {
                return NotFound();
            }

            _applicationDbContext.Automobili.Remove(automobil);
            _applicationDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search(string searchQuery)
        {
            var searchedCars = _applicationDbContext.Automobili
                .Where(c => c.Name.Contains(searchQuery))
                .ToList();

            return View("Index", searchedCars);
        }

        public IActionResult Ponuda()
        {
            var automobili = _applicationDbContext.Automobili.ToList();
            return View(automobili);
        }

        [HttpGet]
        public IActionResult GetPriceByName(string name)
        {
            var automobil = _applicationDbContext.Automobili.FirstOrDefault(a => a.Name == name);
            if (automobil != null)
            {
                return Json(automobil.Price);
            }
            return Json(0);
        }


        [HttpGet]
        public IActionResult SortByPrice(string sortOrder)
        {
            ViewData["PriceSortParam"] = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";

            switch (sortOrder)
            {
                case "price_desc":
                    ViewData["PriceSortParam"] = "price_asc";
                    return View("Index", _applicationDbContext.Automobili.OrderBy(c => c.Price).ToList());
                case "price_asc":
                    ViewData["PriceSortParam"] = "price_desc";
                    return View("Index", _applicationDbContext.Automobili.OrderByDescending(c => c.Price).ToList());
                default:
                    return View("Index", _applicationDbContext.Automobili.OrderByDescending(c => c.Price).ToList());
            }
        }

        [HttpGet]
        public IActionResult SortByName(string sortOrder)
        {
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            var automobili = from car in _applicationDbContext.Automobili select car;
            switch (sortOrder)
            {
                case "name_desc":
                    automobili = automobili.OrderByDescending(c => c.Name);
                    break;
                default:
                    automobili = automobili.OrderBy(c => c.Name);
                    break;
            }
            return View("Index", automobili.ToList());
        }


        [HttpGet]
    public IActionResult Edit(int id)
    {
        var automobil = _applicationDbContext.Automobili.Find(id);
        if (automobil == null)
        {
            return NotFound();
        }

        return View(automobil);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Automobil automobil)
    {
        if (ModelState.IsValid)
        {
            _applicationDbContext.Update(automobil);
            _applicationDbContext.SaveChanges();
            return RedirectToAction("Index");
        }
        return View(automobil);
    }


        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            var automobil = _applicationDbContext.Automobili.Find(id);
            if (automobil == null)
            {
                return NotFound();
            }

            automobil.IsActive = !automobil.IsActive; // Prebacivanje stanja
            _applicationDbContext.Update(automobil);
            _applicationDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
