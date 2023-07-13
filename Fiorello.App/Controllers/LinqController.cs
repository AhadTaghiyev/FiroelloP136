using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.App.Context;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.Controllers
{
    public class LinqController : Controller
    {
        private readonly FiorelloDbContext _context;

        public LinqController(FiorelloDbContext context)
        {
            _context = context;
        }

        //public IActionResult Index()
        //{
        //    //Comment result = _context.Comments.;

        //    //IQueryable<Comment> commentQuery = _context.Comments.Where(x => !x.IsDeleted)
        //    //    .AsNoTrackingWithIdentityResolution();

        //    //IEnumerable<Comment> comments = comment.ToList();

        //    //IEnumerable<Comment> comments =
        //    //    commentQuery.Select(x=>
        //    //    new Comment { Text=x.Text}).ToList();
        //    //foreach (var item in comments)
        //    //{
        //    //    item.Text = "AsnoTracking Islendi";
        //    //}
        //    //_context.SaveChanges();
        //    return Json(result);
        //}
    }
}

