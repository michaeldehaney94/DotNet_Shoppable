using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Shoppable.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IConfiguration configuration;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly decimal shippingFee; // get fee from appsettings

        public CartController( ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager) 
        {
            this.db = db;
            this.configuration = configuration;
            this.userManager = userManager;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
        }



        public IActionResult Index()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, db);
            decimal subtotal = CartHelper.GetSubTotal(cartItems);

            // pass cart data to view
            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            return View();
        }


        //Process checkout
        [Authorize]
        [HttpPost]
        public IActionResult Index(CheckoutDto model)
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, db);
            decimal subtotal = CartHelper.GetSubTotal(cartItems);

            // pass cart data to view
            ViewBag.CartItems = cartItems;
            ViewBag.ShippingFee = shippingFee;
            ViewBag.Subtotal = subtotal;
            ViewBag.Total = subtotal + shippingFee;

            if (ModelState.IsValid) 
            { 
                return View(model);
            }

            // check if shopping cart is empty or not
            if (cartItems.Count == 0)
            {
                ViewBag.ErrorMessage = "Your cart is empty";
                return View(model);
            }

            TempData["DeliveryAddress"] = model.DeliveryAddress;
            TempData["PaymentMethod"] = model.PaymentMethod;

            return RedirectToAction("Confirm");
        }






    }
}
