using System;
using System.Security.Cryptography;
using Fiorello.App.Context;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello.App.Controllers
{
	[Authorize(Roles ="User")]
	public class OrderController:Controller
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly FiorelloDbContext _context;

        public OrderController(UserManager<AppUser> userManager, FiorelloDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> CheckOut()
		{
			AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
			var baskets = await _context.Baskets.Where(x => !x.IsDeleted && x.AppUserId == appUser.Id)
						  .Include(x => x.BasketItems.Where(y => !y.IsDeleted))
						  .ThenInclude(x => x.Product)
						   .ThenInclude(x => x.ProductImages.Where(y => !y.IsDeleted))
						   .Include(x => x.BasketItems.Where(y => !y.IsDeleted))
						   .ThenInclude(x => x.Product)
						   .ThenInclude(x => x.Discount)
							.FirstOrDefaultAsync();

			if (baskets == null || baskets.BasketItems.Count() == 0)
			{
				TempData["empty basket"] = "Basket is empty";
				return RedirectToAction("index", "home");
			}
			return View(baskets);
		}

		public async Task<IActionResult> CreateOrder()
		{
            AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
            var baskets = await _context.Baskets.Where(x => !x.IsDeleted && x.AppUserId == appUser.Id)
                          .Include(x => x.BasketItems.Where(y => !y.IsDeleted))
                          .ThenInclude(x => x.Product)
                           .ThenInclude(x => x.ProductImages.Where(y => !y.IsDeleted))
                           .Include(x => x.BasketItems.Where(y => !y.IsDeleted))
                           .ThenInclude(x => x.Product)
                           .ThenInclude(x => x.Discount)
                            .FirstOrDefaultAsync();
            if (baskets == null || baskets.BasketItems.Count() == 0)
            {
                TempData["empty basket"] = "Basket is empty";
                return RedirectToAction("index", "home");
            }
			Order order = new Order
			{
				AppUserId = appUser.Id,
				CreatedDate = DateTime.Now,
			};
			decimal TotalPrice = 0;
			foreach (var item in baskets.BasketItems)
			{
				TotalPrice += (item.Product.Discount == null ? item.Product.Price * item.ProductCount : (item.Product.Price - (item.Product.Price * (decimal)(item.Product.Discount.Percent / 100))) * item.ProductCount);

                OrderItem orderItem = new OrderItem
				{
					Order = order,
					CreatedDate = DateTime.Now,
					ProductId = item.ProductId,
					ProductCount = item.ProductCount,
				};
                await _context.OrderItems.AddAsync(orderItem);
            }
			order.TotalPrice = TotalPrice;
			await _context.Orders.AddAsync(order);

			baskets.IsDeleted = true;

			await _context.SaveChangesAsync();

            TempData["Order Created"] = "Order succesfuly created";
            return RedirectToAction("index", "home");
        }
	}
}

