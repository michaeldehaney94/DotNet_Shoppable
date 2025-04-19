using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace DotNet_Shoppable.Controllers
{
    // authenticated client users should have access to this controller
    [Authorize(Roles = "client")]
    [Route("/Client/Orders/{action=Index}/{id?}")]
    public class ClientOrdersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly int pageSize = 15; //page size

        // constructor used to connect to the database via models, and read the authenticated user from the user database table
        public ClientOrdersController(ApplicationDbContext db, UserManager<ApplicationUser> userManager) 
        {
            this.db = db;
            this.userManager = userManager;
        }

       
        public async Task<IActionResult> Index(int pageIndex)
        {
            // check if the client is authenticated or logged in
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // include every order item made by the client into the Orders list, when login in as a client
            IQueryable<Order> query = db.Orders.Include(o => o.Items).OrderByDescending(o => o.Id)
                .Where(o => o.ClientId == currentUser.Id);

            // check if page exist
            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            var orders = query.ToList();

            // display orders in view
            ViewBag.Orders = orders;
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;

            return View();
        }

        // order details
        public async Task<IActionResult> Details(int id)
        {
            //check if user is logged in or exist
            var currentUser = await userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var order = db.Orders.Include(o => o.Items).ThenInclude(oi => oi.Product)
                .Where(o => o.ClientId == currentUser.Id).FirstOrDefault(o => o.Id == id);

            // check if order is null
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            return View(order);
        }












    }
}
