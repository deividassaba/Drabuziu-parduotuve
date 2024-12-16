using System.Linq;
using System.Web.Mvc;
using WebApplication2.Models;
using WebApplication2.Data;

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
            // Check if the user is logged in by checking the session
            var userId = Session["UserId"];
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication"); // Redirect to login page if not logged in
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == (int)userId);
            if (user == null)
            {
                return HttpNotFound(); // Handle if user is not found
            }
            /*
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
            */
            return View(user); // Pass user to the view
        }

    }
}
