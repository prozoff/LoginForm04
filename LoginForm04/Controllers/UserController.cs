using LoginForm04.Models;
using LoginForm04.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginForm04.Controllers
{
    public class UsersController : Controller
    {
        SignInManager<User> _signInManager;
        UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index() => View(_userManager.Users.ToList());

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<ActionResult> Block(string[] selectedUsers)
        {
            bool checkSelf = false;
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            if(curUser != null && !curUser.isBlocked)
            {
                for (int i = 0; i < selectedUsers.Length; i++)
                {
                    User user = await _userManager.FindByIdAsync(selectedUsers[i]);
                    if (user != null)
                    {
                        user.isBlocked = true;
                        await _userManager.UpdateAsync(user);
                        if (curUser.Id == selectedUsers[i]) checkSelf = true;
                    }
                }
                if (checkSelf)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login", "Account");
                }
                
            } 
            else
            {
                return RedirectToAction("Login", "Account");
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<ActionResult> Unblock(string[] selectedUsers)
        {
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            if (curUser != null && !curUser.isBlocked)
            {
                for (int i = 0; i < selectedUsers.Length; i++)
                {
                    User user = await _userManager.FindByIdAsync(selectedUsers[i]);
                    user.isBlocked = false;
                    await _userManager.UpdateAsync(user);
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(string[] selectedUsers)
        {
            bool checkSelf = false;
            var curUser = await _userManager.GetUserAsync(HttpContext.User);
            if(curUser != null && !curUser.isBlocked)
            {
                for (int i = 0; i < selectedUsers.Length; i++)
                {
                    User user = await _userManager.FindByIdAsync(selectedUsers[i]);
                    if (user != null)
                    {
                        IdentityResult result = await _userManager.DeleteAsync(user);
                        if (curUser.Id == selectedUsers[i]) checkSelf = true;
                    }

                }
                if (checkSelf)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login", "Account");
                }
                
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("Index");
        }

    }
}
