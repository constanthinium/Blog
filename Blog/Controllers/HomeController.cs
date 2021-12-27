using Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private BlogContext db;

        public HomeController(ILogger<HomeController> logger, BlogContext db)
        {
            _logger = logger;
            this.db = db;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = db.Users.Where(u => u.Email == User.Identity.Name).First();
                return View(user);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ChangeEmail(int id, string email)
        {
            db.Users.Find(id).Email = email;
            db.SaveChanges();
            return Logout();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ChangePassword(int id, string password)
        {
            db.Users.Find(id).Password = password;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}
