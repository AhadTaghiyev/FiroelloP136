using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fiorello.App.ViewModels;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager)
        {
            _userManager = userManager;
            _signinManager = signinManager;
        }

        [Authorize(Roles ="SuperAdmin")]
        public async Task<IActionResult> AdminCreate()
        {
            AppUser SuperAdmin = new AppUser
            {
                Name = "SuperAdmin",
                Surname = "SuperAdmin",
                Email = "SuperAdmin@Mail.ru",
                UserName = "SuperAdmin",
                EmailConfirmed=true
            };
            await _userManager.CreateAsync(SuperAdmin, "Admin123@");
            AppUser Admin = new AppUser
            {
                Name = "Admin",
                Surname = "Admin",
                Email = "Admin@Mail.ru",
                UserName = "Admin",
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(Admin, "Admin123@");

            await _userManager.AddToRoleAsync(SuperAdmin,"SuperAdmin");
            await _userManager.AddToRoleAsync(Admin,"Admin");
            return Json("ok");
        }

        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            AppUser appUser = await _userManager.FindByNameAsync(login.UserName);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Username or password incorrect");
                return View(login);
            }
            Microsoft.AspNetCore.Identity.SignInResult result =
                await _signinManager.PasswordSignInAsync(appUser,login.Password,login.isRememberMe,true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "Your account is blocked for 5 minute");
                    return View(login);
                }
                ModelState.AddModelError("", "Username or password incorrect");
                return View(login);
            }

            return RedirectToAction("index", "home");
        }
        [Authorize(Roles ="Admin,SuperAdmin")]
        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
  
    }
}

