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

    public class PositionController : Controller
    {
        private readonly FiorelloDbContext _context;

        public PositionController(FiorelloDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Position> positions =await
                _context.Positions.Where(x => !x.IsDeleted).ToListAsync();
            return View(positions);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Position position)
        {
            if (!ModelState.IsValid) return View();
            position.CreatedDate = DateTime.Now;
           await _context.Positions.AddAsync(position);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Position? position = await _context.Positions
                          .Where(x => !x.IsDeleted && x.Id == id)
                             .FirstOrDefaultAsync();
            if (position == null)
                return NotFound();

            return View(position);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Position position)
        {
            Position? UpdatePosition = await _context.Positions
                          .Where(x => !x.IsDeleted && x.Id == id)
                             .FirstOrDefaultAsync();
            if (position == null)
                return NotFound();

            if (!ModelState.IsValid) return View(UpdatePosition);

            UpdatePosition.Name = position.Name;
            UpdatePosition.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Position? position = await _context.Positions
                        .Where(x => !x.IsDeleted && x.Id == id)
                           .FirstOrDefaultAsync();
            if (position == null)
                return NotFound();

            position.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}

