﻿using System;
using System.Collections.Generic;
using Fiorello.App.Context;
using Fiorello.App.Services.Interfaces;
using Fiorello.App.ViewModels;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Fiorello.App.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly FiorelloDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AppUser> _userManager;

        public BasketService(FiorelloDbContext context, IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        public async Task AddBasket(int id)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == id))
            {
                throw new Exception("Item not found");
            }
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_httpContext.HttpContext.User.Identity.Name);

                Basket basket = await _context.Baskets.Include(x=>x.BasketItems.Where(y=>!y.IsDeleted)).ThenInclude(x=>x.Product).Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).FirstOrDefaultAsync();

                if (basket == null)
                {
                    basket = new Basket
                    {
                        AppUserId = appUser.Id,
                        CreatedDate = DateTime.Now,
                    };
                    await _context.AddAsync(basket);
                    BasketItem basketItem = new BasketItem
                    {
                        Basket = basket,
                        ProductId = id,
                        ProductCount = 1
                    };
                    await _context.AddAsync(basketItem);
                }
                else
                {
                    BasketItem basketItem = basket.BasketItems.FirstOrDefault(x=>x.ProductId==id);
                    if (basketItem != null)
                    {
                        basketItem.ProductCount++;
                    }
                    else
                    {
                         basketItem = new BasketItem
                        {
                            Basket = basket,
                            ProductId = id,
                            ProductCount = 1
                        };
                        await _context.AddAsync(basketItem);
                    }
                }
                await _context.SaveChangesAsync();

            }
            else
            {
                var CookieJson = _httpContext?.HttpContext?.Request.Cookies["basket"];
                if (CookieJson == null)
                {
                    List<BasketViewModel> basketViewModels = new List<BasketViewModel>();
                    BasketViewModel basketViewModel = new BasketViewModel
                    {
                        ProductId = id,
                        Count = 1
                    };
                    basketViewModels.Add(basketViewModel);
                    CookieJson = JsonConvert.SerializeObject(basketViewModels);

                    _httpContext?.HttpContext?.Response.Cookies.Append("basket", CookieJson);
                }
                else
                {
                    List<BasketViewModel>? basketViewModels = JsonConvert
                                    .DeserializeObject<List<BasketViewModel>>(CookieJson);
                    BasketViewModel? model =
                        basketViewModels.FirstOrDefault(x => x.ProductId == id);
                    if (model != null)
                    {
                        model.Count++;
                    }
                    else
                    {
                        BasketViewModel basketViewModel = new();
                        basketViewModel.Count = 1;
                        basketViewModel.ProductId = id;
                        basketViewModels.Add(basketViewModel);
                    }
                    CookieJson = JsonConvert.SerializeObject(basketViewModels);
                    _httpContext?.HttpContext?.Response.Cookies.Append("basket", CookieJson);
                }
            }
        }

        public async Task<List<BasketItemViewModel>> GetAllBaskets()
        {
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_httpContext.HttpContext.User.Identity.Name);
                Basket? basket = await _context.Baskets.Include(x => x.BasketItems.Where(y => !y.IsDeleted))
                            .ThenInclude(x=>x.Product)
                             .ThenInclude(x=>x.ProductImages)
                             .Include(x=>x.BasketItems)
                             .ThenInclude(x=>x.Product)
                             .ThenInclude(x=>x.Discount)
                               .Where(x => !x.IsDeleted && x.AppUserId == appUser.Id).FirstOrDefaultAsync();

               
              

                if (basket != null)
                {
                    List<BasketItemViewModel> basketItemViewModels = new();
                    foreach (var item in basket.BasketItems)
                    {
                        basketItemViewModels.Add(new BasketItemViewModel
                        {
                            Image = item.Product.ProductImages.FirstOrDefault(x => x.IsMain).Image,
                            Count = item.ProductCount,
                            Name = item.Product.Name,
                            ProductId = item.ProductId,
                            Price = item.Product.Discount == null ? item.Product.Price : (item.Product.Price - (item.Product.Price * ((decimal)item.Product.Discount.Percent / 100)))
                        });
                    }
                    return basketItemViewModels;
                }
            }
            else
            {
                var jsonBasket = _httpContext?.HttpContext?.Request.Cookies["basket"];

                if (jsonBasket != null)
                {
                    List<BasketViewModel>? basketViewModels = JsonConvert
                             .DeserializeObject<List<BasketViewModel>>(jsonBasket);
                    List<BasketItemViewModel> basketItemViewModels = new();
                    foreach (var item in basketViewModels)
                    {
                        Product? product = await _context.Products
                                          .Where(x => !x.IsDeleted && x.Id == item.ProductId)
                                           .Include(x => x.ProductImages)
                                           .Include(x => x.Discount)
                                           .FirstOrDefaultAsync();

                        if (product != null)
                        {
                            basketItemViewModels.Add(new BasketItemViewModel
                            {
                                ProductId = item.ProductId,
                                Count = item.Count,
                                Image = product.ProductImages.FirstOrDefault(x => x.IsMain).Image,
                                Name = product.Name,
                                Price = product.Discount == null ? product.Price : (product.Price - (product.Price * ((decimal)product.Discount.Percent / 100)))
                            });

                        }
                    }
                    return basketItemViewModels;
                }
            }
            return new List<BasketItemViewModel>();
        }

        public async Task Remove(int id)
        {
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.FindByNameAsync(_httpContext.HttpContext.User.Identity.Name);

                Basket? basket = await _context.Baskets.Include(x => x.BasketItems.Where(y => !y.IsDeleted))
                                .Where(x => !x.IsDeleted && x.AppUserId == user.Id).FirstOrDefaultAsync();

      

                var discounts = await _context.Discounts.ToListAsync();


                if (basket != null)
                {
                    BasketItem basketItem = basket.BasketItems.FirstOrDefault(x=>x.ProductId==id);

                    if (basketItem != null)
                    {
                        basketItem.IsDeleted = true;
                        await _context.SaveChangesAsync();
                    }
                }
            }
            else
            {
                var basketJson = _httpContext?.HttpContext?
                           .Request.Cookies["basket"];
                if (basketJson != null)
                {
                    List<BasketViewModel>? basketViewModels = JsonConvert
                             .DeserializeObject<List<BasketViewModel>>(basketJson);

                    BasketViewModel basketViewModel = basketViewModels.FirstOrDefault(x => x.ProductId == id);
                    if (basketViewModel != null)
                    {
                        basketViewModels.Remove(basketViewModel);
                        basketJson = JsonConvert.SerializeObject(basketViewModels);
                        _httpContext?.HttpContext?.Response.Cookies.Append("basket", basketJson);
                    }
                }
            }
        }
    }
}


