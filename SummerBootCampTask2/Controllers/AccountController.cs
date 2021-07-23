using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask2.Contexts;
using SummerBootCampTask2.Services;
using SummerBootCampTask2.CoreModels;
using SummerBootCampTask2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SummerBootCampTask2.Controllers
{
    public class AccountController : Controller
    {
        private readonly BootCampDbContext dbContext;
        private readonly HashService hashService = new HashService();

        public AccountController(BootCampDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Settings()
        {
            var user = dbContext.Users.Where(x => x.Email == User.Identity.Name).ToList();
            SettingsViewModel userModel = new SettingsViewModel()
            {
                UserName = user.First().UserName,
                Email = user.First().Email,
                IsVisible = user.First().IsVisible
            };
            return View(userModel);
        }

        [HttpPost]
        public async Task<IActionResult> Settings(SettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.Users
                       .FirstOrDefault(user => user.Email == User.Identity.Name);

                if (user.Email == model.Email)
                {
                    user.UserName = model.UserName;
                    user.IsVisible = model.IsVisible;
                    if (!String.IsNullOrEmpty(model.Password)) user.Password = hashService.GetHashCode(model.Password);

                    dbContext.SaveChanges();

                    await Authenticate(model.Email);
                }
                else if (!dbContext.Users.Any(user => user.Email == model.Email))
                {
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.IsVisible = model.IsVisible;
                    if (!String.IsNullOrEmpty(model.Password)) user.Password = hashService.GetHashCode(model.Password);
                    
                    dbContext.SaveChanges();

                    await Authenticate(model.Email);
                }
                else
                {
                    ModelState.AddModelError("", "User already exist!");
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = dbContext.Users
                    .FirstOrDefault(user => user.Email == model.Email && user.Password == hashService.GetHashCode(model.Password));

                if (user != null)
                {
                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid password and(or) email");
            }

            return View(model);
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
                var user = dbContext.Users.FirstOrDefault(user => user.Email == model.Email);

                if (user is null)
                {
                    var random = new Random();

                    int identifier;

                    do
                    {
                        identifier = random.Next(1000, 10000);
                    } while (!dbContext.Users.All(user => user.Identifier != identifier));

                    dbContext.Users.Add(new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Password = hashService.GetHashCode(model.Password),
                        Key = random.Next(20),
                        IsVisible = true,
                        Identifier = identifier
                    });

                    dbContext.SaveChanges();

                    await Authenticate(model.Email);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "User already exist!");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, email),
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}
