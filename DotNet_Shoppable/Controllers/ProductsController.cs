using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Shoppable.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment environment; // this will enable you to save an uploaded image in the web app environment or database
        private readonly int pageSize = 5; //pagination page size

        //retrieve database model context
        public ProductsController(ApplicationDbContext db, IWebHostEnvironment environment) 
        {
            this.db = db;
            this.environment = environment;
        }

        // LIST products
        public IActionResult Index(int pageIndex, string? search)
        {
            IQueryable<Product> query = db.Products;

            //search functionality
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search) || p.Category.Contains(search));
            }

            query = query.OrderByDescending(p => p.Id);  // reverse the list order from new to old

            //page indexing
            if (pageIndex < 1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            Console.WriteLine(query.ToString());

            var products = query.ToList();

            // pass page index and page size to the index view
            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

           // add search query value to URL in view
            ViewData["Search"] = search ?? "";

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
       
        //Edit product
        public IActionResult Edit(int id)
        {
            var product = db.Products.Find(id);

            // check if id is null
            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            // create productDto from product
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            //Display product data in edit view
            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }

        //POST Edit product
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            if (!ModelState.IsValid)
            {
                //Display product data in edit view
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

                return View(productDto);
            }

            // update the image file if a new image is uploaded
            string newFileName = product.ImageFileName;
            if (productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath)) 
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                // delete the old image
                string oldImageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);

            }

            // update the product in the database
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            db.SaveChanges();

            return RedirectToAction("Index", "Products");
        }


        //DELETE product
        public IActionResult Delete(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            //delete image from path
            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index", "Products");
        }

    }
}
