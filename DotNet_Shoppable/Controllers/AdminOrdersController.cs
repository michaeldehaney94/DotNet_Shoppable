using DotNet_Shoppable.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Shoppable.Controllers
{
    [Authorize(Roles ="admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext db;

        public AdminOrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            // include ever order item made by a client into the Orders list when login in as admin
            var orders = db.Orders.Include(o => o.Client).Include(o => o.Items).OrderByDescending(o => o.Id).ToList();

            // display orders in view
            ViewBag.Orders = orders;

            return View();
        }
    }
}
