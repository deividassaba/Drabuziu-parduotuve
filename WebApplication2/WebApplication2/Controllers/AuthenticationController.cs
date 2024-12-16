using System;
using System.Web.Mvc;
using WebApplication2.Models;
using System.Linq;
using System.Web.Security;
using System.Data.Entity;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthenticationController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if phone number is already registered in the vartotojas table
                if (_context.Users.Any(u => u.PhoneNumber == model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                    return View(model);
                }

                // Create the user and add it to the vartotojas table
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password, // Make sure to hash the password in a real-world app
                    PhoneNumber = model.PhoneNumber
                };

                _context.Users.Add(user); // Add to vartotojas table
                _context.SaveChanges(); // Commit changes to the database



                return RedirectToAction("Login");
            }

            return View(model);
        }


        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                                          .FirstOrDefaultAsync(u => u.FirstName == model.Username && u.Password == model.Password);

                if (user != null)
                {
                   
                    Session["UserId"] = user.Id;
                    return Redirect("/Profile");

                }
                else
                {
                    ModelState.AddModelError("", "Invalid phone number or password.");
                }
            }

            return View(model);
        }


        public ActionResult Logout()
        {
            Session.Clear(); // Clear the session to log out
            return Redirect("/Profile/View"); // Redirect to the home page or login page
        }

    }
}
