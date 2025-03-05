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

            if (!ModelState.IsValid)
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


        //cart orders
        public IActionResult Confirm()
        {
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, db);
            decimal total = CartHelper.GetSubTotal(cartItems) + shippingFee;
            // get cart size
            int cartSize = 0;
            foreach (var item in cartItems)
            {
                cartSize += item.Quantity;
            }

            string deliveryAddress =  TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep(); // stores the delivery address and payment method as cache for confirming order

            // if cart size, delivery address or payment method is invalid or null
            if (cartSize == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.DeliveryAddress = deliveryAddress; 
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Total = total;
            ViewBag.CartSize = cartSize;


            return View();
        }


        // confirm cart order
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Confirm(int any) // parameter not needed
        {
            var cartItems = CartHelper.GetCartItems(Request, Response, db); // count items added to cart by JavaScript function

            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            string paymentMethod = TempData["PaymentMethod"] as string ?? "";
            TempData.Keep(); // stores the delivery address and payment method as cache for confirming order

            // if cart size, delivery address or payment method is invalid or null
            if (cartItems.Count == 0 || deliveryAddress.Length == 0 || paymentMethod.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // check if use has been authenticated
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // save order
            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                PaymentStatus = "pending",
                PaymentDetails = "",
                OrderStatus = "pending",
                CreatedAt = DateTime.UtcNow,
                //CreatedAt = DateTime.Now, // <==== uncomment when working with sqlserver 
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // delete the shopping cart cookie
            Response.Cookies.Delete("shopping_cart");
            ViewBag.SuccessMessage = "Order created successfully";

            return View();
        }
    }
      
       


    
}
