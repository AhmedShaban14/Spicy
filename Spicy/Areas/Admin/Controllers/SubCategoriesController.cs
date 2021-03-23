using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spicy.Data;
using Spicy.Models.ViewModels;
using Spicy.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Areas.Admin.Controllers
{
    [Authorize(SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        public SubCategoriesController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var subCategories = db.SubCategories.Include(x => x.Category).ToList();
            return View(subCategories);
        }
        public ActionResult GetExistingSub(int id)
        {
            var subs = db.SubCategories.Where(x => x.CategoryId == id);
            return Json(subs);
        }
        [HttpGet]
        public ActionResult Create()
        {
            CategoryAndSubGategoryViewModel model = new CategoryAndSubGategoryViewModel
            {
                Categories = db.Categories.ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(CategoryAndSubGategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subExisting = db.SubCategories.Include(x => x.Category).Where(x => x.Name == model.SubCategory.Name && x.CategoryId == model.SubCategory.CategoryId);
                if (subExisting.Count() > 0)
                {
                    ViewBag.error = "You Already Have This Existed At " + subExisting.FirstOrDefault().Category.Name + " Category";
                }
                else
                {
                    db.SubCategories.Add(model.SubCategory);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubGategoryViewModel modelVm = new CategoryAndSubGategoryViewModel
            {
                Categories = db.Categories.ToList(),
            };
            return View(modelVm);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sub = db.SubCategories.SingleOrDefault(x => x.Id == id);
            CategoryAndSubGategoryViewModel model = new CategoryAndSubGategoryViewModel
            {
                Categories = db.Categories.ToList(),
                SubCategory = sub
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CategoryAndSubGategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subExisting = db.SubCategories.Where(x => x.Name == model.SubCategory.Name && x.CategoryId == model.SubCategory.CategoryId && x.Id != model.SubCategory.Id);
                if (subExisting.Count() > 0)
                {
                    ViewBag.error = "You Already Have This Here";
                }
                else
                {
                    db.SubCategories.Update(model.SubCategory);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            CategoryAndSubGategoryViewModel modelVm = new CategoryAndSubGategoryViewModel
            {
                Categories = db.Categories.ToList(),
            };
            return View(modelVm);
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var sub = db.SubCategories.SingleOrDefault(x => x.Id == id);
            CategoryAndSubGategoryViewModel model = new CategoryAndSubGategoryViewModel
            {
                Categories = db.Categories.ToList(),
                SubCategory = sub
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var sub = db.SubCategories.SingleOrDefault(x => x.Id == id);
            CategoryAndSubGategoryViewModel model = new CategoryAndSubGategoryViewModel
            {
                Categories = db.Categories.ToList(),
                SubCategory = sub
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult ConfirmDelete(CategoryAndSubGategoryViewModel model)
        {
            db.SubCategories.Remove(model.SubCategory);
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
