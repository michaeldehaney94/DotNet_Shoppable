using System.Diagnostics;
using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Shoppable.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext db;

        public HomeController(ApplicationDbContext applicationDbContext)
        {
            this.db = applicationDbContext;
        }

        //List products
        public IActionResult Index()
        {
            var products = db.Products.OrderByDescending(p => p.Id).Take(4).ToList(); //Take() allows you to take the 4 newest products added in descending order
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
