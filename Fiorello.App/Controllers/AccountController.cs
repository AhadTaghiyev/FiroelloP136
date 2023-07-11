using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Fiorello.App.Services.Interfaces;
using Fiorello.App.ViewModels;
using Fiorello.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fiorello.App.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly IWebHostEnvironment _env;
        private readonly IMailService _mailService;

        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, SignInManager<AppUser> signinManager, IWebHostEnvironment env, IMailService mailService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signinManager = signinManager;
            _env = env;
            _mailService = mailService;
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

            string token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
         
            string link = Url.Action(action: "VerifyEmail", controller: "account", values: new { token = token, email = appUser.Email }, protocol: Request.Scheme);

            string text = $"<a href='{link}' id='link-a' target='_blank' style='display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;'>Click for verify email</a>";

            await _mailService.Send("pm4283719@gmail.com",appUser.Email,link, text,"Verify Email");

            TempData["Register"] = "please verify your email.";
            return RedirectToAction("index","home");
        }

        public async Task<IActionResult> VerifyEmail(string token,string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            await _userManager.ConfirmEmailAsync(user,token);
            await _signinManager.SignInAsync(user,true);
            return RedirectToAction(nameof(Info));
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

        [Authorize]
        public async Task<IActionResult> Update()
        {
            var user =await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            UserUpdateViewModel userUpdateView = new UserUpdateViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                UserName = user.UserName
            };
            return View(userUpdateView);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(UserUpdateViewModel model)
        {
            
            var user =await _userManager.FindByNameAsync(User.Identity.Name);
            if (!ModelState.IsValid) return View(model);
            if (user == null)
            {
                return NotFound();
            }
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.UserName = model.UserName;
            user.Email = model.Email;

          var result=  await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description);
                }
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
               result= await _userManager.ChangePasswordAsync(user,model.CurrentPassword,model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("",item.Description);
                    }
                    return View(model);
                }
            }

            await _signinManager.SignInAsync(user,true);
            TempData["updateduser"] = "ok";
            return RedirectToAction(nameof(Info));
        }
        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            var link = Url.Action(action: "resetpassword", controller: "account", values: new {token= token,email=email}, protocol: Request.Scheme);
            string text = $"<a href='{link}' id='link-a' target='_blank'" +
                $" style='display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro'," +
                $" Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none;" +
                $" border-radius: 6px;'>Click me for reset password</a>";

            await _mailService.Send("pm4283719@gmail.com", user.Email, link, text, "Reset Password");

            return RedirectToAction("Index","home");
        }
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token,string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }
            ResetPasswordViewModel resetPasswordViewModel = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(resetPasswordViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPassword)
        {
            var user = await _userManager.FindByEmailAsync(resetPassword.Email);

            if (user == null)
            {
                return NotFound();
            }
           var result= await _userManager.ResetPasswordAsync(user, resetPassword.Token,resetPassword.Password);
            if (!result.Succeeded)
            {
                return Json(result.Errors);
            }
            return RedirectToAction("login","account");
        }



    }
}

