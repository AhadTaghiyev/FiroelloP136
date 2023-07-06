using System;
using System.Collections.Generic;
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
    [Authorize(Roles ="Admin,SuperAdmin")]
    public class BlogController : Controller
    {
        private readonly FiorelloDbContext _context;
        private IWebHostEnvironment _env;

        public BlogController(FiorelloDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await
                   _context.Blogs
                      .Where(x => !x.IsDeleted).ToListAsync();
            return View(blogs);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Blog blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (blog.FormFile == null)
            {
                ModelState.AddModelError("FormFile","The filed image is required");
                return View();
            }

            if (!Helper.IsImage(blog.FormFile))
            {
                ModelState.AddModelError("FormFile","The file type must be image");
                return View();
            }
            if (!Helper.IsSizeOk(blog.FormFile,1))
            {
                ModelState.AddModelError("FormFile", "The file size can not than more 1 mb");
                return View();
            }

            blog.Image = blog.FormFile.CreateImage(_env.WebRootPath,"assets/images");
            blog.CreatedDate = DateTime.Now;
            await _context.AddAsync(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int id)
        {
            Blog? blog = await _context.Blogs
                           .Where(x => !x.IsDeleted && x.Id == id)
                               .FirstOrDefaultAsync();

            if (blog == null)
                return NotFound();

            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id,Blog blog)
        {
            Blog? Updateblog = await _context.Blogs
                      .Where(x => !x.IsDeleted && x.Id == id)
                          .FirstOrDefaultAsync();

            if (blog == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(Updateblog);
            }

       

            if (blog.FormFile != null)
            {
                if (!Helper.IsImage(blog.FormFile))
                {
                    ModelState.AddModelError("FormFile", "The file type must be image");
                    return View();
                }
                if (!Helper.IsSizeOk(blog.FormFile, 1))
                {
                    ModelState.AddModelError("FormFile", "The file size can not than more 1 mb");
                    return View();
                }

                Helper.RemoveImage(_env.WebRootPath,"assets/images",Updateblog.Image);

                Updateblog.Image = blog.FormFile
                           .CreateImage(_env.WebRootPath,"assets/images");

            }

            Updateblog.Description = blog.Description;
            Updateblog.Title = blog.Title;
            Updateblog.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Blog? blog = await _context.Blogs
                        .Where(x => !x.IsDeleted && x.Id == id)
                            .FirstOrDefaultAsync();

            if (blog == null)
                return NotFound();

            //Helper.RemoveImage(_env.WebRootPath,"assets/images",blog.Image);

            blog.IsDeleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

