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

    public class DiscountController : Controller
    {
        private readonly FiorelloDbContext _context;

        public DiscountController(FiorelloDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Discount> Discounts =
                await _context.Discounts.Where(x => !x.IsDeleted).ToListAsync();
            return View(Discounts);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Discount Discount)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            await _context.AddAsync(Discount);
            await _context.SaveChangesAsync();
            return RedirectToAction("index","Discount");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            Discount? Discount = await _context.Discounts
                .Where(x => !x.IsDeleted && x.Id == id)
                                 .FirstOrDefaultAsync();
            if (Discount == null)
                return NotFound();

            return View(Discount);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Discount postDiscount)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Discount? Discount = await _context.Discounts
                 .Where(x => !x.IsDeleted && x.Id == id)
                                  .FirstOrDefaultAsync();
            if (Discount == null)
                return NotFound();

            Discount.Percent = postDiscount.Percent;
            await _context.SaveChangesAsync();
            return RedirectToAction("index","Discount");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Discount? Discount = await _context.Discounts
              .Where(x => !x.IsDeleted && x.Id == id)
                               .FirstOrDefaultAsync();
            if (Discount == null)
                return NotFound();

            //_context.Discounts.Remove(Discount);
            Discount.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

