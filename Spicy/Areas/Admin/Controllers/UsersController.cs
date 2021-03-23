using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spicy.Data;
using Spicy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spicy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.ManagerUser)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        public UsersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var claimIdentity =(ClaimsIdentity) User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var usersList = db.ApplicationUser.Where(x => x.Id != userId).ToList();
            return View(usersList);
        }

        public IActionResult LockUnLock(string? id)
        {
            if (id == null)
                return NotFound();
            var user = db.ApplicationUser.SingleOrDefault(x => x.Id == id);
            if(user ==null)
                return NotFound();

            if(user.LockoutEnd ==null || user.LockoutEnd<DateTime.Now)
            {
                user.LockoutEnd=DateTime.Now.AddHours(4);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
