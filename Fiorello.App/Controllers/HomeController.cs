using System.Diagnostics;
using System.Text.Json.Serialization;
using Fiorello.App.Context;
using Fiorello.App.ViewModels;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiorello.App.Controllers;

public class HomeController : Controller
{
    private readonly FiorelloDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(FiorelloDbContext context, UserManager<AppUser> userManager)
    {

        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity.IsAuthenticated)
        {
            var jsonBasket = Request.Cookies["basket"];
            if (jsonBasket != null)
            {
                AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
                Basket? basket = await _context.Baskets.Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).FirstOrDefaultAsync();
                if (basket == null)
                {
                     basket = new Basket
                    {
                        CreatedDate = DateTime.Now,
                        AppUser = appUser,
                    };
                    await _context.Baskets.AddAsync(basket);
                }
                List<BasketViewModel> viewModels = JsonConvert.DeserializeObject<List<BasketViewModel>>(jsonBasket);
                foreach (var item in viewModels)
                {
                    BasketItem basketItem = new BasketItem
                    {
                        Basket = basket,
                        CreatedDate = DateTime.Now,
                        ProductId = item.ProductId,
                        ProductCount = item.Count
                    };
                    await _context.BasketItems.AddAsync(basketItem);
                }
                await _context.SaveChangesAsync();

                Response.Cookies.Delete("basket");
            }
        }
      
        HomeViewModel homeViewModel = new HomeViewModel
        {
           
            Categories = await _context.Categories
            .Where(x => !x.IsDeleted).ToListAsync(),
            Blogs = await _context.Blogs
              .Where(x => !x.IsDeleted).ToListAsync(),
            Employees = await _context.Employees
                  .Include(x => x.Position)
               .Where(x => !x.IsDeleted).ToListAsync(),
            Products = await _context.Products.Where(x => !x.IsDeleted)
            .Include(x => x.ProductImages)
            .Include(x => x.Discount)
             .Include(x => x.ProductCategories)

             .ThenInclude(x => x.Category)
              .ToListAsync()
        };

           
        return View(homeViewModel);
    }

 
}

