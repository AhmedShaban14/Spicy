using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Spicy.Data;
using Spicy.Models;
using Spicy.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spicy.Areas.Admin.Controllers
{
    //[Authorize(SD.ManagerUser)]
    [Area("Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hosting;

        public CouponsController(ApplicationDbContext db,IWebHostEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }

        public IActionResult Index()
        {
            var coupons = db.Coupons.ToList();
            return View(coupons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View( );
        }

        [HttpPost]
        public IActionResult Create(Coupon c)
        {
            if (ModelState.IsValid)
            {
                string fileName = "";
                if (c.File != null)
                {
                     fileName = Guid.NewGuid() + c.File.FileName;

                    string upload = Path.Combine(hosting.WebRootPath, "Coupons");
                    string fullPath = Path.Combine(upload, fileName);
                    FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                    c.File.CopyTo(fileStream);
                }
                c.ImageUrl = fileName;
                db.Coupons.Add(c);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(c);
        }



        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var couponDb = db.Coupons.SingleOrDefault(x => x.Id == id); 

            return View(couponDb);
        }

        [HttpPost]
        public IActionResult Edit(Coupon c)
        {
            if (ModelState.IsValid)
            {
                string fileName = c.ImageUrl;
                if (c.File != null)
                {
                    fileName = Guid.NewGuid() + c.File.FileName;

                    string upload = Path.Combine(hosting.WebRootPath, "Coupons");
                    string fullPath = Path.Combine(upload, fileName);
                    FileStream fileStream = new FileStream(fullPath, FileMode.Create);
                    c.File.CopyTo(fileStream);
                }
                c.ImageUrl = fileName;
                db.Coupons.Update(c);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(c);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var couponDb = db.Coupons.SingleOrDefault(x => x.Id == id);

            return View(couponDb);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var couponDb = db.Coupons.SingleOrDefault(x => x.Id == id);

            return View(couponDb);
        }

        [HttpPost]
        public IActionResult ConfirmDelete(int? id)
        {
            if (id == null)
                return NotFound();
            var couponDb = db.Coupons.SingleOrDefault(x => x.Id == id);
            db.Remove(couponDb);
            db.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
