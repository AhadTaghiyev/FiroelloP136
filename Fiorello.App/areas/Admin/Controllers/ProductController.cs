using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.App.Context;
using Fiorello.App.Extentions;
using Fiorello.App.Helpers;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class ProductController : Controller
    {
        private readonly FiorelloDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(FiorelloDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await
                 _context.Products.Where(x => !x.IsDeleted).Include(x=>x.ProductImages).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories =await _context.Categories
                .Where(x => !x.IsDeleted)
                   .ToListAsync();
            ViewBag.Tags = await _context.Tags
                .Where(x => !x.IsDeleted)
                   .ToListAsync();
            ViewBag.Discounts = await _context.Discounts
             .Where(x => !x.IsDeleted)
                .ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Categories = await _context.Categories
                .Where(x => !x.IsDeleted)
                   .ToListAsync();
            ViewBag.Tags = await _context.Tags
                .Where(x => !x.IsDeleted)
                   .ToListAsync();
            ViewBag.Discounts = await _context.Discounts
             .Where(x => !x.IsDeleted)
                .ToListAsync();

            if (!ModelState.IsValid)
                return View(product);
            if (product.FormFiles==null||product.FormFiles.Count == 0)
            {
                ModelState.AddModelError("","min image count 1");
                return View(product);
            }
            int i = 0;
            product.DiscountId = product.DiscountId == 0 ? null : product.DiscountId;
            foreach (var item in product.FormFiles)
            {
                if (!Helper.IsImage(item))
                {
                    ModelState.AddModelError("", "it is not image");
                    return View(product);
                }
                if (!Helper.IsSizeOk(item,1))
                {
                    ModelState.AddModelError("", "image size must be less 1");
                    return View(product);
                }


                ProductImage productImage = new ProductImage
                {
                    CreatedDate = DateTime.Now,
                    Image = item.CreateImage(_env.WebRootPath, "assets/images"),
                    Product=product,
                    IsMain=i==0?true:false
                };
                i++;
                //product.ProductImages.Add(productImage); buda olar
                await _context.ProductImages.AddAsync(productImage);
            }

            foreach (var item in product.CategoryIds)
            {
                if(!await _context.Categories.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("","agilli ol");
                    return View(product);
                }
                ProductCategory productCategory = new ProductCategory
                {
                    CategoryId = item,
                    Product = product,
                    CreatedDate=DateTime.Now
                };
                await _context.ProductCategories.AddAsync(productCategory);
            }

            foreach (var item in product.TagIds)
            {
                if (!await _context.Tags.AnyAsync(x => x.Id == item))
                {
                    ModelState.AddModelError("", "agilli ol");
                    return View(product);
                }
                ProductTag productTag = new ProductTag
                {
                    TagId = item,
                    Product = product,
                    CreatedDate = DateTime.Now
                };
                await _context.ProductTags.AddAsync(productTag);
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Categories = await _context.Categories
              .Where(x => !x.IsDeleted)
                 .ToListAsync();
            ViewBag.Tags = await _context.Tags
                .Where(x => !x.IsDeleted)
                   .ToListAsync();
            ViewBag.Discounts = await _context.Discounts
             .Where(x => !x.IsDeleted)
                .ToListAsync();
            Product? product = await _context.Products
                       .Where(x => !x.IsDeleted&&x.Id==id)
                        .Include(x=>x.ProductImages.Where(x=>!x.IsDeleted))
                        .Include(x=>x.Discount)
                         .Include(x=>x.ProductCategories.Where(x=>!x.IsDeleted))
                          .ThenInclude(x=>x.Category)
                            .Include(x => x.ProductTags.Where(x => !x.IsDeleted))
                          .ThenInclude(x => x.Tag)
                        .FirstOrDefaultAsync();
            return View(product);
        }

        public async Task<IActionResult> SetAsMainImage(int id)
        {
            ProductImage productImage =await _context.ProductImages.FindAsync(id);

            if (productImage == null)
            {
                return Json(new { status = 404 });
            }

            productImage.IsMain = true;
            ProductImage? productImage1 = await _context.ProductImages
                        .Where(x => x.IsMain&&x.ProductId==productImage.ProductId).FirstOrDefaultAsync();
            productImage1.IsMain = false;
            await _context.SaveChangesAsync();
            return Json(new {status=200});
        }

        public async Task<IActionResult> RemoveImage(int id)
        {
            ProductImage? productImage = await _context
                .ProductImages.Where(x => !x.IsDeleted && x.Id == id)
                  .FirstOrDefaultAsync();

            if (productImage == null)
                return Json(new { status = 404,desc="image not found" });

            if (productImage.IsMain)
                return Json(new { status = 400, desc = "can not remove main imae" });

            productImage.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Json(new { status=200});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Product product)
        {
            ViewBag.Categories = await _context.Categories
              .Where(x => !x.IsDeleted)
                 .ToListAsync();
            ViewBag.Tags = await _context.Tags
                .Where(x => !x.IsDeleted)
                   .ToListAsync();
            ViewBag.Discounts = await _context.Discounts
             .Where(x => !x.IsDeleted)
                .ToListAsync();

            Product? update = await _context.Products
                .AsNoTrackingWithIdentityResolution()
                 .Where(x => !x.IsDeleted && x.Id == id)
                  .Include(x => x.ProductImages.Where(x => !x.IsDeleted))
                  .Include(x => x.Discount)
                   .Include(x => x.ProductCategories.Where(x => !x.IsDeleted))
                    .ThenInclude(x => x.Category)
                      .Include(x => x.ProductTags.Where(x => !x.IsDeleted))
                    .ThenInclude(x => x.Tag)
                  .FirstOrDefaultAsync();

            product.DiscountId = product.DiscountId == 0 ? null : product.DiscountId;
            if (update == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(update);
            }
    
            List<ProductTag> RemoveableTag = await _context.ProductTags.Where(x => !product.TagIds.Contains(x.TagId))
                                                 .ToListAsync();
         
                _context.ProductTags.RemoveRange(RemoveableTag);

            foreach (var item in product.TagIds)
            {
                if (_context.ProductTags.Where(x => x.ProductId == id && x.TagId == item).Count() > 0)
                    continue;

                await _context.ProductTags.AddAsync(new ProductTag
                {
                    ProductId = id,
                    TagId = item
                }) ;
            }

            List<ProductCategory> RemoveableCategory = await _context.ProductCategories.Where(x => !product.CategoryIds.Contains(x.CategoryId))
                                         .ToListAsync();

            _context.ProductCategories.RemoveRange(RemoveableCategory);

            foreach (var item in product.CategoryIds)
            {
                if (_context.ProductCategories.Where(x => x.ProductId == id && x.CategoryId == item).Count() > 0)
                    continue;

                await _context.ProductCategories.AddAsync(new ProductCategory
                {
                    ProductId = id,
                    CategoryId = item
                });
            }

            if (product.FormFiles != null && product.FormFiles.Count > 0)
            {

                foreach (var item in product.FormFiles)
                {
                    if (!Helper.IsImage(item))
                    {
                        ModelState.AddModelError("", "it is not image");
                        return View(update);
                    }
                    if (!Helper.IsSizeOk(item, 1))
                    {
                        ModelState.AddModelError("", "image size must be less 1");
                        return View(update);
                    }


                    ProductImage productImage = new ProductImage
                    {
                        CreatedDate = DateTime.Now,
                        Image = item.CreateImage(_env.WebRootPath, "assets/images"),
                        Product = product,
                    };
                    //product.ProductImages.Add(productImage); buda olar
                    await _context.ProductImages.AddAsync(productImage);
                }

            }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product? product = await _context
                            .Products.Where(x => !x.IsDeleted && x.Id == id)
                            .FirstOrDefaultAsync();
            if (product == null)
                return NotFound();
            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

