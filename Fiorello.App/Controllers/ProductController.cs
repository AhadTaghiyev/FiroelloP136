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
                      .Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync(),
                Products = await _context.Products
                       .Include(x => x.ProductImages.Where(x => !x.IsDeleted)).Take(4)
                        .Where(x => !x.IsDeleted).ToListAsync()
            };
          
            return View(productViewModel);
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            await _basketService.AddBasket(id);
            return RedirectToAction("index","home");
        }
        public async Task<IActionResult> RemoveBasket(int id)
        {
            await _basketService.Remove(id);
            return RedirectToAction("index", "home");
        }
    }
}

