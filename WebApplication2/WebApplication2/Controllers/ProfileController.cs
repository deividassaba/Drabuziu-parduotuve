using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Data;
using System;

namespace WebApplication2.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DatabaseContext _dbContext;

        public ProfileController()
        {
            _context = new ApplicationDbContext();
            _dbContext = new DatabaseContext();

        }

        // GET: Profile/View
        public ActionResult View()
        {
            var userId = Session["UserId"];

            if (userId == null)
            {
                return RedirectToAction("Error");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == (int)userId);
            if (user == null)
            {
                return RedirectToAction("Error");
            }

            // Check if the user is already in the Buyers or Sellers table
            var existingBuyer = _context.Buyers.FirstOrDefault(b => b.Id == (int)userId);
            var existingSeller = _context.Sellers.FirstOrDefault(s => s.Id == (int)userId);

            bool isBuyer = existingBuyer != null;
            bool isSeller = existingSeller != null;

            ViewBag.IsBuyer = isBuyer;
            ViewBag.IsSeller = isSeller;
            ViewBag.ShowRoleSelection = !isBuyer && !isSeller;

            var couponId = _context.Database.SqlQuery<int?>(
                "SELECT fk_nuolaidoskodas FROM vartotojas WHERE id = @p0",
                (int)userId
                ).FirstOrDefault();

            if (couponId.HasValue)
			{
                ViewBag.Coupon = _dbContext.Coupons
                    .Include("CouponProducts")
                    .FirstOrDefault(c => c.Id == couponId);
			}
            
            return View(user); // Pass user to the view
        }
        // POST: SelectRole (Assign user to Buyer or Seller with default data)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectRole(string role)
        {
            var userId = Session["UserId"];

            if (userId == null)
            {
                return RedirectToAction("Error");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == (int)userId);
            if (user == null)
            {
                return RedirectToAction("Error");
            }

            // Ensure the user has not already selected a role
            var existingBuyer = _context.Buyers.FirstOrDefault(b => b.Id == (int)userId);
            var existingSeller = _context.Sellers.FirstOrDefault(s => s.Id == (int)userId);

            if (existingBuyer != null || existingSeller != null)
            {
                return RedirectToAction("View"); // Already has a role, redirect to profile
            }

            // Add the user to the appropriate table based on the selected role
            if (role == "Buyer")
            {
                var buyer = new Buyers
                {
                    Id = (int)userId,
                    Birthday = GenerateRandomDate(), // Use the helper method to generate a random DateTime
                    Place = "Default Place", // Default place
                    Credit = 1000.0, // Default credit
                    Gender = 1 // Default gender (can be adjusted)
                };
                _context.Buyers.Add(buyer);
            }
            else if (role == "Seller")
            {
                var seller = new Sellers
                {
                    Id = (int)userId,
                    Place = "Default Place" // Default place for seller
                };
                _context.Sellers.Add(seller);
                _context.SaveChanges();
            }

            

            return RedirectToAction("View"); // Redirect to the updated profile page
        }
        // Helper method to generate a random DateTime within a range
        private DateTime GenerateRandomDate()
        {
            Random rand = new Random();
            int year = rand.Next(1950, 2000); // Random year between 1950 and 2000
            int month = rand.Next(1, 13); // Random month between 1 and 12
            int day = rand.Next(1, 29); // Random day between 1 and 28 (to avoid issues with different month lengths)

            return new DateTime(year, month, day);
        }
    }
}
