using System.Diagnostics;
using Fiorello.App.Context;
using Fiorello.App.ViewModels;
using Fiorello.Core.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.App.Controllers;

public class HomeController : Controller
{
    private readonly FiorelloDbContext _context;

    public HomeController(FiorelloDbContext context)
    {
        
          _context = context;
    }

    public async Task<IActionResult> Index()
    {
        //ViewBag.Color = Request.Cookies["color"];
      
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
            .Include(x=>x.Discount)
             .Include(x => x.ProductCategories)

             .ThenInclude(x => x.Category)
              .ToListAsync()
        };

           
        return View(homeViewModel);
    }

    //public async Task<IActionResult> ChangeColor(string color)
    //{
    //    var colorcookie = Request.Cookies["color"];

    //    if (string.IsNullOrWhiteSpace(colorcookie))
    //    {
    //        Response.Cookies.Append("color",color);
    //    }
    //    else
    //    {
    //        Response.Cookies.Delete("color");
    //        Response.Cookies.Append("color", color);
    //    }
    //    return RedirectToAction("index","home");
    //}
 
}

