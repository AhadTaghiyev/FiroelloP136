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

namespace Fiorello.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerView)
        {
            if (!ModelState.IsValid)
            {
                return View(registerView);
            }

            AppUser appUser = new AppUser
            {
                Name = registerView.Name,
                Surname = registerView.Surname,
                UserName = registerView.UserName,
                Email = registerView.Email,
            };
          IdentityResult identityResult=  await _userManager.CreateAsync(appUser,registerView.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("",item.Description);
                }
                return View(registerView);
            }
            await _userManager.AddToRoleAsync(appUser,"User");

            return RedirectToAction("index","home");
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

        public async Task<IActionResult> Logout()
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [Authorize]
        public async Task<IActionResult> Info()
        {
            string username = User.Identity.Name;
            AppUser appUser = await _userManager.FindByNameAsync(username);
            return View(appUser);
        }
        //public async Task<IActionResult> CreateRole()
        //{
        //    IdentityRole identityRole1 = new IdentityRole { Name = "SuperAdmin" };
        //    IdentityRole identityRole2 = new IdentityRole { Name = "Admin" };
        //    IdentityRole identityRole3 = new IdentityRole { Name = "User" };

        //    await _roleManager.CreateAsync(identityRole1);
        //    await _roleManager.CreateAsync(identityRole2);
        //    await _roleManager.CreateAsync(identityRole3);
        //    return Json("ok");
        //}
    }
}

