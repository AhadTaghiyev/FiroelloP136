using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.App.Context;
using Fiorello.App.Extentions;
using Fiorello.App.Helpers;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeController : Controller
    {
        private readonly FiorelloDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(FiorelloDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Employee> employees = await _context.Employees
                   .Include(x=>x.Position)
                       .Where(x => !x.IsDeleted).ToListAsync();
            return View(employees);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Positions =await _context.Positions.Where(x => !x.IsDeleted)
                                   .ToListAsync();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            ViewBag.Positions = await _context.Positions.Where(x => !x.IsDeleted)
                               .ToListAsync();
            if (!ModelState.IsValid) return View();

            if (employee.FormFile == null)
            {
                ModelState.AddModelError("FormFile","Image is required");
                return View();
            }
            if (!Helper.IsImage(employee.FormFile))
            {
                ModelState.AddModelError("FormFile", "Image is must be an image");
                return View();
            }
            if (!Helper.IsSizeOk(employee.FormFile,1))
            {
                ModelState.AddModelError("FormFile", "Image size can not than more 1mb");
                return View();
            }

            employee.Image = employee.FormFile.CreateImage(_env.WebRootPath,"assets/images");


            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Positions = await _context.Positions.Where(x => !x.IsDeleted)
                                      .ToListAsync();
            Employee? employee = await _context.Employees
                     .Where(x => !x.IsDeleted&&x.Id==id).FirstOrDefaultAsync();
            if (employee == null) return NotFound();
            return View(employee);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(int id,Employee employee)
        {
            ViewBag.Positions = await _context.Positions.Where(x => !x.IsDeleted)
                                      .ToListAsync();
            Employee? Updateemployee = await _context.Employees
                     .Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
            if (employee == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(Updateemployee);
            }

            if (employee.FormFile != null)
            {
                if (!Helper.IsImage(employee.FormFile))
                {
                    ModelState.AddModelError("FormFile", "Image is must be an image");
                    return View(Updateemployee);
                }
                if (!Helper.IsSizeOk(employee.FormFile, 1))
                {
                    ModelState.AddModelError("FormFile", "Image size can not than more 1mb");
                    return View(Updateemployee);
                }
                Updateemployee.Image= employee.FormFile.CreateImage(_env.WebRootPath, "assets/images");
            }

            Updateemployee.PositionId = employee.PositionId;
            Updateemployee.FullName = employee.FullName;
            Updateemployee.UpdatedDate = DateTime.Now;
            Updateemployee.Description = employee.Description;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

