using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using M_RENT_Aplikacija.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace M_RENT_Aplikacija.Controllers
{
    public class UserRolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Method to handle search by name
        public async Task<IActionResult> SearchByName(string searchName)
        {
            var usersQuery = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchName))
            {
                usersQuery = usersQuery.Where(u => u.UserName.Contains(searchName));
            }

            var users = await usersQuery.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = rolesForUser.ToList()
                });
            }

            ViewData["SearchName"] = searchName;
            ViewData["SearchPhone"] = "";
            ViewData["SearchRole"] = "";
            ViewData["Roles"] = roles;

            return View("Index", userRoles);
        }

        // Method to handle search by phone number
        public async Task<IActionResult> SearchByPhone(string searchPhone)
        {
            var usersQuery = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(searchPhone))
            {
                usersQuery = usersQuery.Where(u => u.PhoneNumber.Contains(searchPhone));
            }

            var users = await usersQuery.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = rolesForUser.ToList()
                });
            }

            ViewData["SearchName"] = "";
            ViewData["SearchPhone"] = searchPhone;
            ViewData["SearchRole"] = "";
            ViewData["Roles"] = roles;

            return View("Index", userRoles);
        }

        public async Task<IActionResult> SearchByRole(string searchRole)
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = new List<UserRolesViewModel>();
            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);
                if (!string.IsNullOrEmpty(searchRole) && rolesForUser.Any(r => r.ToLower() == searchRole.ToLower()))
                {
                    userRoles.Add(new UserRolesViewModel
                    {
                        UserId = user.Id,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = rolesForUser.ToList()
                    });
                }
            }

            ViewData["SearchName"] = "";
            ViewData["SearchPhone"] = "";
            ViewData["SearchRole"] = searchRole;
            ViewData["Roles"] = roles;

            return View("Index", userRoles);
        }


        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoles = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var rolesForUser = await _userManager.GetRolesAsync(user);

                userRoles.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = rolesForUser.ToList()
                });
            }

            ViewData["SearchName"] = "";
            ViewData["SearchPhone"] = "";
            ViewData["SearchRole"] = "";
            ViewData["Roles"] = roles;

            return View(userRoles);
        }
    }
}
