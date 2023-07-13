using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.App.Context;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class CategoryController : Controller
    {
        private readonly FiorelloDbContext _context;

        public CategoryController(FiorelloDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page=1)
        {
            int TotalCount = _context.Categories.Where(x => !x.IsDeleted).Count();

            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 5);
            ViewBag.CurrentPage = page;

            IEnumerable <Category> categories =
                await _context.Categories.Where(x => !x.IsDeleted).Skip((page-1)*5).Take(5).ToListAsync();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            await _context.AddAsync(category);
            await _context.SaveChangesAsync();
            return RedirectToAction("index","category");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Category? category = await _context.Categories
                .Where(x => !x.IsDeleted && x.Id == id)
                                 .FirstOrDefaultAsync();
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Category postcategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category? category = await _context.Categories
                 .Where(x => !x.IsDeleted && x.Id == id)
                                  .FirstOrDefaultAsync();
            if (category == null)
                return NotFound();

            category.Name = postcategory.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("index","category");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Category? category = await _context.Categories
              .Where(x => !x.IsDeleted && x.Id == id)
                               .FirstOrDefaultAsync();
            if (category == null)
                return NotFound();

            //_context.Categories.Remove(category);
            category.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

