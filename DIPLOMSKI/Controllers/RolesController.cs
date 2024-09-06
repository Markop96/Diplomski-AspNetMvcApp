using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace M_RENT_Aplikacija.Controllers
{
  
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _manager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _manager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _manager.Roles;
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(IdentityRole role)
        {
            if (!_manager.RoleExistsAsync(role.Name).GetAwaiter().GetResult())
            {
                _manager.CreateAsync(new IdentityRole(role.Name)).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var role = _manager.FindByIdAsync(id).GetAwaiter().GetResult();
            if (role != null)
            {
                var result = _manager.DeleteAsync(role).GetAwaiter().GetResult();
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Failed to delete role");
            }
            else
            {
                ModelState.AddModelError("", "Role not found");
            }
            return View("Index", _manager.Roles);
        }
    }
}
