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

    public class OrderController : Controller
    {
        private readonly FiorelloDbContext _context;

        public OrderController(FiorelloDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Order> Orders =
                await _context.Orders.Where(x => !x.IsDeleted)
                .Include(x=>x.AppUser)
                .Include(x=>x.OrderItems)
                .ThenInclude(x=>x.Product)
                .ThenInclude(x=>x.ProductImages)
                .ToListAsync();
            return View(Orders);
        }

        public async Task<IActionResult> Accept(int id)
        {
            Order? order = await _context.Orders.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            order.IsAccepted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Reject(int id)
        {
            Order? order = await _context.Orders.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            order.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> complete(int id)
        {
            Order? order = await _context.Orders.Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            order.IsCompleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}

