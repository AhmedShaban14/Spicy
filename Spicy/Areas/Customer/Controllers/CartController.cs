using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spicy.Data;
using Spicy.Models;
using Spicy.Models.ViewModels;
using Spicy.Utility;
using Stripe;
using System;
using System.Linq;
using System.Security.Claims;

namespace Spicy.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;


        public CartController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [BindProperty]
        public OrderHeaderShoppingCartViewModel CartDetail { get; set; }
        public IActionResult Index()
        {
            CartDetail = new OrderHeaderShoppingCartViewModel
            {
                OrderHeader = new Models.OrderHeader()

            };
            //Get USer Id :
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;


            var shoppingCarts = db.ShoppingCarts.Where(x => x.ApplicationUserId == userId);
            if (shoppingCarts != null)
            {
                CartDetail.ShoppingCarts = shoppingCarts.ToList();

            }

            foreach (var item in CartDetail.ShoppingCarts)
            {
                item.MenuItem = db.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);

                var orderTotal = item.MenuItem.Price * item.Count;
                CartDetail.OrderHeader.OrderTotal += orderTotal;

                item.MenuItem.Description = SD.ConvertToRawHtml(item.MenuItem.Description);
            }

            CartDetail.OrderHeader.OrderTotalOriginal = CartDetail.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                CartDetail.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponInDb = db.Coupons.SingleOrDefault(x => x.Name.ToLower() == CartDetail.OrderHeader.CouponCode.ToLower());

                CartDetail.OrderHeader.OrderTotal = SD.DiscountPrice(couponInDb, CartDetail.OrderHeader.OrderTotalOriginal);
            }

            return View(CartDetail);
            ////Get Current User Id:
            //var claimsIdentity = (ClaimsIdentity)User.Identity;
            //var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //var userId = claims.Value;
            //OrderHeaderShoppingCartViewModel vm = new OrderHeaderShoppingCartViewModel
            //{
            //    OrderHeader = new Models.OrderHeader()
            //};
            //var shoppingCart = db.ShoppingCarts.Where(x => x.ApplicationUserId == userId).ToList();
            //if (shoppingCart != null)
            //{
            //    vm.ShoppingCarts = shoppingCart;
            //    vm.OrderHeader.OrderTotal = 0;

            //    foreach (var item in shoppingCart)
            //    {
            //        item.MenuItem = db.MenuItems.SingleOrDefault(x => x.Id == item.MenuItemId);

            //        vm.OrderHeader.OrderTotal = item.Count * item.MenuItem.Price;
            //    }
            //    vm.OrderHeader.OrderTotalOriginal = vm.OrderHeader.OrderTotal;
            //}
            //return View(vm);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            CartDetail = new OrderHeaderShoppingCartViewModel
            {
                OrderHeader = new Models.OrderHeader()

            };
            //Get USer Id :
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            //Get User Info : 
            var appUser = db.ApplicationUser.SingleOrDefault(x => x.Id == userId);
            CartDetail.OrderHeader.PickUpName = appUser.NameOfUser;
            CartDetail.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            CartDetail.OrderHeader.PickUpDate = DateTime.Now;



            var shoppingCarts = db.ShoppingCarts.Where(x => x.ApplicationUserId == userId);
            if (shoppingCarts != null)
            {
                CartDetail.ShoppingCarts = shoppingCarts.ToList();

            }

            foreach (var item in CartDetail.ShoppingCarts)
            {
                item.MenuItem = db.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);

                var orderTotal = item.MenuItem.Price * item.Count;
                CartDetail.OrderHeader.OrderTotal += orderTotal;

            }

            CartDetail.OrderHeader.OrderTotalOriginal = CartDetail.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                CartDetail.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponInDb = db.Coupons.SingleOrDefault(x => x.Name.ToLower() == CartDetail.OrderHeader.CouponCode.ToLower());

                CartDetail.OrderHeader.OrderTotal = SD.DiscountPrice(couponInDb, CartDetail.OrderHeader.OrderTotalOriginal);
            }

            return View(CartDetail);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            //Get USer Id :
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            //Get User Info : 
            //var appUser = db.ApplicationUser.SingleOrDefault(x => x.Id == userId);

            CartDetail.OrderHeader.UserId =userId;
            CartDetail.OrderHeader.OrderDate = DateTime.Now;
            CartDetail.OrderHeader.PickUpTime = Convert.ToDateTime(CartDetail.OrderHeader.OrderDate.ToShortDateString()+" "+
                 CartDetail.OrderHeader.PickUpTime.ToShortTimeString());
            CartDetail.OrderHeader.Status = SD.PaymentStatusPending;
            CartDetail.OrderHeader.PaymentStatus = SD.PaymentStatusPending;

            db.OrderHeaders.Add(CartDetail.OrderHeader);
            db.SaveChanges();


            CartDetail.ShoppingCarts = db.ShoppingCarts.Where(x => x.ApplicationUserId == userId).ToList();
           

            foreach (var item in CartDetail.ShoppingCarts)
            {
                item.MenuItem = db.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);

                OrderDetails orderDetails = new OrderDetails { 
                MenuItemId =item.MenuItemId,
                OrderId = CartDetail.OrderHeader.Id,
                Count=item.Count,
                Name =  item.MenuItem.Name,
                Description =  item.MenuItem.Description,
                Price =  item.MenuItem.Price
                
                };
                var orderTotal = item.MenuItem.Price * item.Count;
                CartDetail.OrderHeader.OrderTotalOriginal += orderTotal;

                db.OrderDetails.Add(orderDetails);
                db.SaveChanges();
               
            }

            CartDetail.OrderHeader.OrderTotalOriginal += CartDetail.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                CartDetail.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponInDb = db.Coupons.SingleOrDefault(x => x.Name.ToLower() == CartDetail.OrderHeader.CouponCode.ToLower());

                CartDetail.OrderHeader.OrderTotal = SD.DiscountPrice(couponInDb, CartDetail.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                CartDetail.OrderHeader.OrderTotal = CartDetail.OrderHeader.OrderTotalOriginal;
            }
            CartDetail.OrderHeader.CouponCodeDiscount = Math.Round(CartDetail.OrderHeader.OrderTotalOriginal - CartDetail.OrderHeader.OrderTotal,2);
            db.ShoppingCarts.RemoveRange(CartDetail.ShoppingCarts);
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, 0);
            //Stripe Config:
            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(CartDetail.OrderHeader.OrderTotal * 100),
                Currency = "usd",
                Description = "Order ID : " + CartDetail.OrderHeader.Id,
                Source = stripeToken

            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                CartDetail.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                CartDetail.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "succeeded")
            {
                CartDetail.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                CartDetail.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                CartDetail.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        public IActionResult AddCoupon()
        {

            if (CartDetail.OrderHeader.CouponCode == null)
            {
                CartDetail.OrderHeader.CouponCode = "";
            }

            HttpContext.Session.SetString(SD.ssCouponCode, CartDetail.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Plus(int itemId)
        {
            var shoppingCarts = db.ShoppingCarts.SingleOrDefault(x => x.Id == itemId);
            shoppingCarts.Count += 1;
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int itemId)
        {
            var shoppingCarts = db.ShoppingCarts.SingleOrDefault(x => x.Id == itemId);
            if (shoppingCarts.Count > 1)
            {
                shoppingCarts.Count -= 1;
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int itemId)
        {
            var shoppingCarts = db.ShoppingCarts.SingleOrDefault(x => x.Id == itemId);
            db.ShoppingCarts.Remove(shoppingCarts);
            db.SaveChanges();
            var count = db.ShoppingCarts.Where(x => x.ApplicationUserId == shoppingCarts.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCartCount,count);
            return RedirectToAction(nameof(Index));
        }
    }
}
