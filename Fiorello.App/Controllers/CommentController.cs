using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.App.Context;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.Controllers
{
    [Authorize(Roles ="User")]
    public class CommentController : Controller
    {
        private readonly FiorelloDbContext _context;
        private readonly UserManager<AppUser> _userManageer;

        public CommentController(FiorelloDbContext context, UserManager<AppUser> userManageer)
        {
            _context = context;
            _userManageer = userManageer;
        }
        [HttpPost]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            AppUser appUser = await _userManageer.FindByNameAsync(User.Identity.Name);
            comment.AppUserId = appUser.Id;
            comment.CreatedDate = DateTime.UtcNow.AddHours(4);
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }
    }
}

