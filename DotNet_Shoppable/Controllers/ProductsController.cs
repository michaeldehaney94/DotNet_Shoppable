using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Shoppable.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment environment; // this will enable you to save an uploaded image in the web app environment or database

        //retrieve database model context
        public ProductsController(ApplicationDbContext db, IWebHostEnvironment environment) 
        {
            this.db = db;
            this.environment = environment;
        }

        public IActionResult Index()
        {
            var products = db.Products.OrderByDescending(p => p.Id).ToList(); // reverse the list order from new to old
            return View(products);
        }

        //GET Create View products
        public IActionResult Create()
        {
            return View();
        }

        //POST Create product
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }

            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            //save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName); //get file extension name from image file 
            //save image file to filepath provided
            string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

           
            //save the new product in the database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreatedAt = DateTime.UtcNow,
                //CreatedAt = DateTime.Now, // works with sql server timestamp format
            };

            db.Products.Add(product);
            db.SaveChanges();

            //return View();
            return RedirectToAction("Index", "Products");
        }
       









    }
}
