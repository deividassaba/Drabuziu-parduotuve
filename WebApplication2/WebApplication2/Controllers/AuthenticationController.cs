using System;
using System.Web.Mvc;
using WebApplication2.Models;
using System.Linq;
using System.Web.Security;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Diagnostics;

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
                // Check if the phone number is already registered in the Users table
                if (_context.Users.Any(u => u.PhoneNumber == model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                    return View(model);
                }

                // Create the new user and add it to the Users table
                var user = new Users
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Password = model.Password, // In a real application, remember to hash the password!
                    PhoneNumber = model.PhoneNumber
                };

                _context.Users.Add(user);
                _context.SaveChanges(); // Save to get the generated Id

                // Check if the logged-in user is an admin and is trying to register a new admin
                var currentUserId = user.Id;
                
                if (_context.Administrators.Count() ==0) // If logged-in user is an admin
                {
                    // Register the new user as an admin
                    var newAdmin = new Administrators
                    {
                        Id = user.Id,
                        Chief = true,  // Set Chief as true for the new admin
                        Salary = 0,    // Example salary
                        Card = "0000"  // Example card number
                    };

                    _context.Administrators.Add(newAdmin);
                    _context.SaveChanges(); // Save the administrator entry
                }
                if (Session["UserId"] != null)
                {
                    var currentloggedinUserId = (int)Session["UserId"];
                    var currentUserAdmin = _context.Administrators.FirstOrDefault(a => a.Id == currentloggedinUserId);

                    if (currentUserAdmin != null && currentUserAdmin.Chief) // If logged-in user is an admin is an admin
                    {
                        // Register the new user as an admin
                        var newAdmin = new Administrators
                        {
                            Id = user.Id,
                            Chief = true,  // Set Chief as true for the new admin
                            Salary = 0,    // Example salary
                            Card = "0000"  // Example card number
                        };

                        _context.Administrators.Add(newAdmin);
                        _context.SaveChanges(); // Save the administrator entry
                    }
                }
                // Redirect to the login page after successful registration
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
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users
                                   .FirstOrDefault(u => u.FirstName == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Store the user ID in the session
                    Session["UserId"] = user.Id;

                    // Check if the user is an admin and set the ViewBag.IsAdmin accordingly
                    var admin = _context.Administrators.FirstOrDefault(a => a.Id == user.Id);
                    ViewBag.IsAdmin = admin != null && admin.Chief;

                    // Set TempData to pass the value to the view
                    TempData["IsAdmin"] = ViewBag.IsAdmin;

                    // Redirect to the home page or root URL after successful login
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }


        public ActionResult Logout()
        {
            Session.Clear(); // Clear the session to log out
            return Redirect("/"); // Redirect to the home page or login page
        }
    }

}
