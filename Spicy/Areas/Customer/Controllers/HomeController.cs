using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spicy.Data;
using Spicy.Models;
using Spicy.Models.ViewModels;
using Spicy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spicy.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            HomePageViewModel vm = new HomePageViewModel { 
            Categories = db.Categories.ToList(),
            Coupons = db.Coupons.Where(x=>x.IsActice==true).ToList(),
            MenuItems = db.MenuItems.Include(x=>x.Category).Include(x=>x.SubCategory).ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult Details(int itemId)
        {

            var menuItem = db.MenuItems.Include(x => x.Category).Include(x => x.SubCategory)
                .SingleOrDefault(x => x.Id == itemId);
            ShoppingCart shoppingCart = new ShoppingCart
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };
            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity =(ClaimsIdentity) User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claim.Value;
                shoppingCart.ApplicationUserId = userId;
                var shoppingDb = db.ShoppingCarts.Where(x => x.ApplicationUserId == shoppingCart.ApplicationUserId && x.MenuItemId == shoppingCart.MenuItemId)
                    .FirstOrDefault();
                if(shoppingDb ==null)
                {
                    //Add New Shopping Cart
                    db.ShoppingCarts.Add(shoppingCart);
                }
                else
                {
                    shoppingDb.Count += shoppingCart.Count;
                }
                db.SaveChanges();
                var count = db.ShoppingCarts
                    .Where(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);
                return RedirectToAction(nameof(Index));
            }
            //ShoppingCart shoppingCart = new ShoppingCart
            //{
            //    MenuItem = menuItem,
            //    MenuItemId = menuItem.Id
            //};
            return View(shoppingCart);
        }

    }
}
