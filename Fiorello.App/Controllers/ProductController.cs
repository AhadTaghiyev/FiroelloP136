using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fiorello.App.Context;
using Fiorello.App.Services.Implementations;
using Fiorello.App.Services.Interfaces;
using Fiorello.App.ViewModels;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.Controllers
{
    public class ProductController : Controller
    {
        private readonly FiorelloDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IBasketService _basketService;

        public ProductController(FiorelloDbContext context, IHttpContextAccessor httpContext, IBasketService basketService)
        {
            _context = context;
            _httpContext = httpContext;
            _basketService = basketService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            IEnumerable<Product> products = default;

            if (id == null || id == 0)
            {
                products= await _context.Products.Where(x => !x.IsDeleted)
                .Include(x => x.ProductImages.Where(x => !x.IsDeleted && x.IsMain))
                 .Include(x => x.Discount).ToListAsync();
            }
            else
            {
                
                products = await _context.Products.Where(x => !x.IsDeleted && x.ProductCategories.Any(y=>y.CategoryId==id))
                .Include(x => x.ProductImages.Where(x => !x.IsDeleted && x.IsMain))
                 .Include(x => x.Discount)
                 .Include(x=>x.ProductCategories)
                 .ToListAsync();
            }


            return View(products);
        }

        public async Task<IActionResult> Detail(int id)
        {
            ProductViewModel productViewModel = new ProductViewModel
            {
                Product = await _context.Products
                      .Include(x => x.ProductImages.Where(x => !x.IsDeleted))
                       .Include(x => x.ProductTags)
                        .ThenInclude(x => x.Tag)
                        .Include(x => x.ProductCategories)
                        .ThenInclude(x => x.Category)
                        .Include(x=>x.Comments)
                        .ThenInclude(x=>x.AppUser)
                      .Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync(),
                Products = await _context.Products
                       .Include(x => x.ProductImages.Where(x => !x.IsDeleted)).Take(4)
                        .Where(x => !x.IsDeleted).ToListAsync()
            };
          
            return View(productViewModel);
        }

        public async Task<IActionResult> AddBasket(int id,int? count)
        {
            await _basketService.AddBasket(id,count);
           
            return Json(new {status=200});
        }
        public async Task<IActionResult> GetAllBaskets()
        {
            var result = await _basketService.GetAllBaskets();
            return Json(result);
        }
        public async Task<IActionResult> RemoveBasket(int id)
        {
            await _basketService.Remove(id);
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}

