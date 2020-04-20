using LoginForm04.Models;
using LoginForm04.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LoginForm04.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string dateNow = DateTime.Now.ToString();
                User user = new User {UserName = model.UserName, Email = model.Email, createDate = dateNow, isBlocked = false, lastLogin = dateNow };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                
               
                
                if (result.Succeeded)
                {
                    string loginDate = DateTime.Now.ToString();
                    User user = await _userManager.FindByNameAsync(model.Username);
                    if (user.isBlocked)
                    {
                        await _signInManager.SignOutAsync();
                        return RedirectToAction("Login", "Account");

                    }
                    user.lastLogin = loginDate;
                    await _userManager.UpdateAsync(user);

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }

                }

                else
                {
                    ModelState.AddModelError("", "Uncorrect login or password");
                }

            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UsersList()
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            if (curUser !=null && !curUser.isBlocked)
            {
                return RedirectToAction("Index", "Users");
            }
            else
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }
            
        }

    }

}
