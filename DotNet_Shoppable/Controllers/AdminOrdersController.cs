using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
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
        private readonly int pageSize = 15; //page size

        public AdminOrdersController(ApplicationDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index(int pageIndex)
        {
            // include every order item made by a client into the Orders list when login in as admin
            //var orders = db.Orders.Include(o => o.Client).Include(o => o.Items).OrderByDescending(o => o.Id).ToList();
            IQueryable<Order> query = db.Orders.Include(o => o.Client).Include(o => o.Items).OrderByDescending(o => o.Id);

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
        public IActionResult Details(int id)
        {
            var order = db.Orders.Include(o => o.Client).Include(o => o.Items).ThenInclude(oi => oi.Product).FirstOrDefault(o => o.Id == id);

            // check if order is null
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            // count orders
            ViewBag.NumOrders = db.Orders.Where(o => o.ClientId == order.ClientId).Count();

            return View(order);
        }


        // edit order statuses
        public IActionResult Edit(int id, string? payment_status, string? order_status)
        {
            var order = db.Orders.Find(id); // if id for order statuses
            if (order == null)
            {
                return RedirectToAction("Index");
            }

            // check if payment or order status is null
            if (payment_status == null && order_status == null)
            {
                return RedirectToAction("Details", new { id });
            }

            if (payment_status != null)
            {
                order.PaymentStatus = payment_status;
            }

            if (order_status != null)
            {
                order.OrderStatus = order_status;
            }

            // save status change
            db.SaveChanges();


            return RedirectToAction("Details", new { id });
        }


    }
}
