using Microsoft.AspNetCore.Mvc;
using SummerBootCampTask2.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SummerBootCampTask2.Controllers
{
    public class UserController : Controller
    {
        private readonly BootCampDbContext dbContext;

        public UserController(BootCampDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string searchedUser)
        {
            var users = dbContext.Users.Where(x => x.IsVisible == true).ToList();

            if (User.Identity.IsAuthenticated)
            {
                users = users.Where(x => x.Email != User.Identity.Name).ToList();
            }

            if (!String.IsNullOrEmpty(searchedUser))
            {
                ViewData["GetUserDeatails"] = searchedUser;

                users = dbContext.Users.Where(x => x.Email != User.Identity.Name && x.Identifier.ToString() == searchedUser).ToList();
            }

            return View(users);
        }
    }
}
