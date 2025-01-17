using DotNet_Shoppable.Models;
using System.Text.Json;

namespace DotNet_Shoppable.Data
{
    public class CartHelper
    {
        public static Dictionary<int, int> GetCartDictionary(HttpRequest request, HttpResponse response) 
        {
            string cookieValue = request.Cookies["shopping_cart"] ?? "";

            try
            {
                var cart = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(cookieValue)); // decode base64 data to string value
                Console.WriteLine("[CartHelper] cart=" + cookieValue + " -> " + cart);
                var dictionary = JsonSerializer.Deserialize<Dictionary<int, int>>(cart); // convert from Json to dictionary integer value
                if (dictionary != null)
                {
                    return dictionary;
                }
            }
            catch (Exception)
            {

                // do not add a "throw;" 
            }

            if (cookieValue.Length > 0)
            {
                // this cookie is not valid => delete it
                response.Cookies.Delete("shopping_cart");
            }

            return new Dictionary<int, int>(); // if dictionary is null, return empty dictionary
        }


        public static int GetCartSize(HttpRequest request, HttpResponse response)
        {
            int cartSize = 0;

            var cartDictionary = GetCartDictionary(request, response);
            foreach (var keyValuePair in cartDictionary)
            {
                cartSize += keyValuePair.Value;
            }

            return cartSize;
        }

        // method used to import cart items and cart dictionary count
       public static List<OrderItem> GetCartItems(HttpRequest request, HttpResponse response, ApplicationDbContext db)
        {
            var cartItems = new List<OrderItem>();

            var cartDictionary = GetCartDictionary(request, response);

            foreach (var pair in cartDictionary)
            {
                int productId = pair.Key;
                int quantity = pair.Value;
                var product = db.Products.Find(productId);
                if (product == null) { continue; }

                var item = new OrderItem
                {
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    Product = product,
                };

                cartItems.Add(item);
            }

            return cartItems;
        }

        // calculate the sub-total of the shopping cart
        public static decimal GetSubTotal(List<OrderItem> cartItems)
        {
            decimal subtotal = 0;

            foreach (var item in cartItems)
            {
                subtotal += item.Quantity * item.UnitPrice;
            }

            return subtotal;
        }







    }
}

// This class will assist in displaying the quantity size of the cart on the server by retrieving the shopping cart cookie value. 

