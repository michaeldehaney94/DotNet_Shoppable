using DotNet_Shoppable.Data;
using DotNet_Shoppable.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Shoppable.Controllers
{
    // The 'Route' parameter tells the controller that 'ProductsController' actions and views should only be available to the administrator
    //[Route("/Admin/[controller]/[action]")]
    [Authorize(Roles = "admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")] // removes the action method name from URL when navigating
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
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = db.Products;

            //search functionality
            if (search != null)
            {
                query = query.Where(p => p.Name.Contains(search) || p.Brand.Contains(search) || p.Category.Contains(search)
                || p.Name.ToLower().Contains(search) || p.Brand.ToLower().Contains(search) || p.Category.ToLower().Contains(search)); //check if the search is lowrcase characters
            }
           

            //sort functionality
            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAt" };
            string[] validOrderBy = { "desc", "asc" };

            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!validOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "CreatedAt")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAt);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAt);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    // reverse the list order from new to old
                    query = query.OrderByDescending(p => p.Id);
                }
            }

            

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

            //add sort column query to URL in view
            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;

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
                //CreatedAt = DateTime.Now, // <==== uncomment when working with sqlserver 
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
