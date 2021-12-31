using Blog.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var user = db.Users.Where(u => u.Email == User.Identity.Name)
                .Include(u => u.Posts).First();
            return View(user);
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

        public IActionResult Post(string text, int userId)
        {
            db.Posts.Add(new Post(text, userId));
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(int id)
        {
            db.Posts.Remove(db.Posts.Find(id));
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            return View(db.Posts.Find(id));
        }

        [HttpPost]
        public IActionResult Edit(int id, string text)
        {
            db.Posts.Find(id).Text = text;
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Search(string name)
        {
            if (name == null) return RedirectToAction(nameof(Index));
            else return View(db.Users.Where(u => u.Email.Contains(name)));
        }

        public IActionResult Profile(int id)
        {
            return View(db.Users.Include(u => u.Posts).First(u => u.Id == id));
        }

        public IActionResult Admin(string result = "")
        {
            return View(Tuple.Create(db.Model.GetEntityTypes(), result));
        }

        public IActionResult Query(string query)
        {
            try
            {
                var conn = db.Database.GetDbConnection();
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText = query;
                var reader = command.ExecuteReader();
                var builder = new StringBuilder();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    builder.Append(reader.GetName(i));
                    builder.Append('\t');
                }
                builder.AppendLine();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var data = reader[i];
                        builder.Append(data.ToString());
                        builder.Append('\t');
                    }
                    builder.AppendLine();
                }
                return RedirectToAction(nameof(Admin), routeValues: new { result = builder.ToString() });
            }
            catch
            {
                return RedirectToAction(nameof(Admin));
            }
        }
    }
}
