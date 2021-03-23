using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spicy.Data;
using Spicy.Models.ViewModels;
using Spicy.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Areas.Admin.Controllers
{
    [Authorize(SD.ManagerUser)]

    [Area("Admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hosting;

        public MenuItemsController(ApplicationDbContext db, IWebHostEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var menuListItems = db.MenuItems.Include(x => x.Category).Include(x => x.SubCategory).ToList();
            return View(menuListItems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            MenuItemsViewModel model = new MenuItemsViewModel
            {
                Categories = db.Categories.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(MenuItemsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = @"defaultPicture.png";
                if (model.MenuItem.File != null)
                {
                    string uploads = Path.Combine(hosting.WebRootPath, "Uploads");
                    fileName = Guid.NewGuid() + model.MenuItem.File.FileName;
                    string fullPath = Path.Combine(uploads, fileName);
                    FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                    model.MenuItem.File.CopyTo(fileStream);
                }
                model.MenuItem.ImageUrl = fileName;
                db.MenuItems.Add(model.MenuItem);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            MenuItemsViewModel modelVm = new MenuItemsViewModel
            {
                Categories = db.Categories.ToList()
            };
            return View(modelVm);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {

            var menuItemDb = db.MenuItems.SingleOrDefault(x => x.Id == id);

            MenuItemsViewModel model = new MenuItemsViewModel
            {
                MenuItem = menuItemDb,
                Categories = db.Categories.ToList(),
                SubCategories = db.SubCategories.Where(x => x.CategoryId == menuItemDb.CategoryId)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(MenuItemsViewModel model)
        {
            if (ModelState.IsValid)
            {
                string newFileName = model.MenuItem.ImageUrl;
                string oldPath = Path.Combine(hosting.WebRootPath, "Uploads");
                string oldPathFileName = Path.Combine(oldPath, model.MenuItem.ImageUrl);

                if (model.MenuItem.File != null)
                {
                   System.IO.File.Delete(oldPathFileName);
                   string uploads = Path.Combine(hosting.WebRootPath, "Uploads");
                   newFileName = Guid.NewGuid() + model.MenuItem.File.FileName;
                   string fullPath = Path.Combine(uploads, newFileName);
                   FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                   model.MenuItem.File.CopyTo(fileStream);
                }
                model.MenuItem.ImageUrl = newFileName;
                db.MenuItems.Update(model.MenuItem);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {

            var menuItemDb = db.MenuItems.SingleOrDefault(x => x.Id == id);

            MenuItemsViewModel model = new MenuItemsViewModel
            {
                MenuItem = menuItemDb,
                Categories = db.Categories.ToList(),
                SubCategories = db.SubCategories.Where(x => x.CategoryId == menuItemDb.CategoryId)
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {

            var menuItemDb = db.MenuItems.SingleOrDefault(x => x.Id == id);

            MenuItemsViewModel model = new MenuItemsViewModel
            {
                MenuItem = menuItemDb,
                Categories = db.Categories.ToList(),
                SubCategories = db.SubCategories.Where(x => x.CategoryId == menuItemDb.CategoryId)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(MenuItemsViewModel model)
        {

            db.MenuItems.Remove(model.MenuItem);
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
