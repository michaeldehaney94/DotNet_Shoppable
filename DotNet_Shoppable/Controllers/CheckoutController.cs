using Azure.Core;
using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json.Nodes;

namespace DotNet_Shoppable.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private string PaypalClientId { get; set; } = "";
        private string PaypalSecret { get; set; } = "";
        private string PaypalUrl { get; set; } = "";

        private readonly decimal shippingFee;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;

        public CheckoutController(IConfiguration configuration, ApplicationDbContext db, UserManager<ApplicationUser> userManager) 
        {
            PaypalClientId = configuration["PaypalSettings:ClientId"]!;
            PaypalSecret = configuration["PaypalSettings:Secret"]!;
            PaypalUrl = configuration["PaypalSettings:Url"]!;
            shippingFee = configuration.GetValue<decimal>("CartSettings:ShippingFee");
            this.db = db;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            // retrieve cart items and items total cost
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, db);
            decimal total = CartHelper.GetSubTotal(cartItems) + shippingFee;

            //read the delivery address from cart
            string deliveryAddress = TempData["DeliveryAddress"] as string ?? "";
            TempData.Keep();

            ViewBag.DeliveryAddress = deliveryAddress; // display delivery address in view
            ViewBag.Total = total; // display total in view

            ViewBag.PaypalClientId = PaypalClientId; // receive JavaScript paypal button via client id
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> CreateOrder(/*[FromBody] JsonObject data*/)
        {
            //var totalAmount = data?["amount"]?.ToString();
            //if (totalAmount == null)
            //{
            //    return new JsonResult(new { Id = "" });
            //}

            // retrieve cart items and items total cost
            List<OrderItem> cartItems = CartHelper.GetCartItems(Request, Response, db);
            decimal totalAmount = CartHelper.GetSubTotal(cartItems) + shippingFee;


            // create the request body
            JsonObject createOrderRequest = new JsonObject();
            createOrderRequest.Add("intent", "CAPTURE");

            JsonObject amount = new JsonObject();
            amount.Add("currency_code", "USD");
            amount.Add("value", totalAmount);

            JsonObject purchaseUnit1 = new JsonObject();
            purchaseUnit1.Add("amount", amount);

            JsonArray purchaseUnits = new JsonArray();
            purchaseUnits.Add(purchaseUnit1);

            createOrderRequest.Add("purchase_units", purchaseUnits);

            // get access token
            string accessToken = await GetPaypalAccessToken();

            // send request
            string url = PaypalUrl + "/v2/checkout/orders";

            // http client
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent(createOrderRequest.ToString(), null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                // check if the request sent was successful
                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderId = jsonResponse["id"]?.ToString() ?? "";

                        return new JsonResult(new { Id = paypalOrderId });
                    }
                }
            }

            return new JsonResult(new { Id = "" });
        }


        // Finish order process
        [HttpPost]
        public async Task<JsonResult> CompleteOrder([FromBody] JsonObject data)
        {
            var orderId = data?["orderID"]?.ToString();
            var deliveryAddress = data?["deliveryAddress"]?.ToString();

            if (orderId == null || deliveryAddress == null)
            {
                return new JsonResult("error");
            }

            // get access token
            string accessToken = await GetPaypalAccessToken();


            string url = PaypalUrl + "/v2/checkout/orders/" + orderId + "/capture";

            // http client
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("", null, "application/json");

                var httpResponse = await client.SendAsync(requestMessage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();
                    var jsonResponse = JsonNode.Parse(strResponse);

                    if (jsonResponse != null)
                    {
                        string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
                        if (paypalOrderStatus == "COMPLETED")
                        {
                            // save the order in the database
                            await SaveOrder(jsonResponse.ToString(), deliveryAddress);

                            return new JsonResult("success");
                        }
                    }
                }
            }


            return new JsonResult("error");
        }

        // Save completed order to database
        private async Task SaveOrder(string paypalResponse, string deliveryAddress)
        {
            // get cart items
            var cartItems = CartHelper.GetCartItems(Request, Response, db);

            // check if user is logged in
            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return;
            }

            // save the order
            var order = new Order
            {
                ClientId = appUser.Id,
                Items = cartItems,
                ShippingFee = shippingFee,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = "paypal",
                PaymentStatus = "accepted",
                PaymentDetails = paypalResponse,
                OrderStatus = "pending",
                CreatedAt = DateTime.UtcNow,
                //CreatedAt = DateTime.Now, // <==== uncomment when working with sqlserver
            };

            db.Orders.Add(order);
            db.SaveChanges();


            // delete the shopping cart cookie after checkout
            Response.Cookies.Delete("shopping_cart");
        }

        // Test token request
        /*public async Task<string> Token()
        {
            return await GetPaypalAccessToken();
        }*/

        // Get paypal access token for request authorization
        private async Task<string> GetPaypalAccessToken()
        {
            string accessToken = "";

            string url = PaypalUrl + "/v1/oauth2/token";

            using (var client = new HttpClient())
            {
                string credentials64 =
                    Convert.ToBase64String(Encoding.UTF8.GetBytes(PaypalClientId + ":" + PaypalSecret));

                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credentials64);

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                requestMessage.Content = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

                var httpResponse = await client.SendAsync(requestMessage);


                if (httpResponse.IsSuccessStatusCode)
                {
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();

                    var jsonResponse = JsonNode.Parse(strResponse);
                    if (jsonResponse != null)
                    {
                        accessToken = jsonResponse["access_token"]?.ToString() ?? "";
                    }
                }
            }


            return accessToken;
        }




    }
}
