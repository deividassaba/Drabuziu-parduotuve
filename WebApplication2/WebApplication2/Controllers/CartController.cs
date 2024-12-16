using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";
        private const string CouponSessionKey = "AppliedCoupon";

        // GET: Cart
        public ActionResult Index()
		{
			var cart = Session[CartSessionKey] as List<CartItem> ?? new List<CartItem>();
			var couponCode = Session[CouponSessionKey] as string;

			// Reset prices to original before applying discount
			foreach (var item in cart)
			{
				item.Price = item.OriginalPrice;
			}
			
			if (!string.IsNullOrEmpty(couponCode))
			{
				using (var db = new DatabaseContext())
				{
					var coupon = db.Coupons
						.Include("CouponProducts")
						.FirstOrDefault(c => c.Kodas == couponCode &&
										   (!c.Galiojimo_pabaigos_data.HasValue || c.Galiojimo_pabaigos_data >= DateTime.Now) &&
										   (!c.Veikimo_pradzios_data.HasValue || c.Veikimo_pradzios_data <= DateTime.Now) &&
										   (!c.Yra_ribotas || (c.Yra_ribotas && c.Panaudojimu_sk > 0)));

					if (coupon != null)
					{
						foreach (var item in cart)
						{
							var couponProduct = coupon.CouponProducts
								.FirstOrDefault(cp => cp.ProductId == item.ProductId);

							if (couponProduct != null && 
								(!couponProduct.MinQuantity.HasValue || item.Quantity >= couponProduct.MinQuantity.Value))
							{
								item.Price = item.OriginalPrice * (1 - (float)(coupon.Verte / 100));
							}
						}
						ViewBag.CouponMessage = "Nuolaida pritaikyta";
					}
					else
					{
						ViewBag.CouponMessage = "Neteisingas nuolaidos kodas arba jis nebegalioja";
						Session[CouponSessionKey] = null;
					}
				}
			}

			return View(cart);
		}

        [HttpPost]
		public ActionResult ApplyCoupon(string couponCode)
		{
			Session[CouponSessionKey] = couponCode;
			return RedirectToAction("Index");
		}


        // POST: AddToCart
        [HttpPost]
        public ActionResult AddToCart(int id, string name, float price)
        {
            var cart = Session[CartSessionKey] as List<CartItem> ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem
                {
                    ProductId = id,
                    Name = name,
                    Price = price,
                    OriginalPrice = price,
                    Quantity = 1
                });
            }

            Session[CartSessionKey] = cart;

            return RedirectToAction("Index");
        }

        // POST: Checkout
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Checkout()
		{
			var cart = Session[CartSessionKey] as List<CartItem>;
			if (cart == null || !cart.Any())
			{
				return RedirectToAction("Index", "Orders");
			}

			var userId = (int)Session["UserId"];

			using (var db = new DatabaseContext())
			using (var userDb = new ApplicationDbContext())
			{
				var order = new Order
				{
					location = "Customer's Address",
					itemCount = cart.Sum(c => c.Quantity),
					cost = (float)cart.Sum(c => c.Price * c.Quantity),
					start = DateTime.Now,
					end = DateTime.Now.AddDays(3),
					status = "Pateiktas",
					buyerId = userId,
					OrderProduct = cart.Select(c => new OrderProduct
					{
						ProductId = c.ProductId,
						cost = (float)(c.Price * c.Quantity)
					}).ToList()
				};

				db.Orders.Add(order);
				db.SaveChanges();

				try
				{
					var coupon = new Coupon
					{
						Sukurimo_data = DateTime.UtcNow,
						Veikimo_pradzios_data = DateTime.UtcNow,
						Galiojimo_pabaigos_data = DateTime.UtcNow.AddMonths(1), // Coupon valid for 1 month
						Panaudojimu_sk = 1, // One-time use
						Yra_ribotas = true,
						Verte = 10, // 10% discount
						Pavadinimas = "Lojalumo nuolaida",
						Aprasymas = "Ačiū už pirkinį! Štai jūsų nuolaida kitam apsipirkimui."
					};

					var random = new Random();
					string code;
					do
					{
						string timestamp = DateTime.UtcNow.ToString("yyMMddHHmm");
						string randomPart = new string(Enumerable.Range(0, 4)
							.Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"[random.Next(36)])
							.ToArray());
						code = $"{timestamp}{randomPart}";
					}
					while (db.Coupons.Any(c => c.Kodas == code));

					coupon.Kodas = code;

					db.Coupons.Add(coupon);
					db.SaveChanges();

					// Link products to the coupon
					// Strategy: Include products that were bought in quantity of 2 or more
					foreach (var item in cart.Where(x => x.Quantity >= 2))
					{
						db.Database.ExecuteSqlCommand(
							"INSERT INTO nuolaidoskodas_produktas (fk_produktas, fk_nuolaidoskodas, minkiekis) VALUES ({0}, {1}, {2})",
							item.ProductId, coupon.Id, 1
						);
					}

					var user = userDb.Users.Find(userId);
					if (user != null)
					{
						userDb.Database.ExecuteSqlCommand(
							"UPDATE vartotojas SET fk_nuolaidoskodas = {0} WHERE id = {1}",
							coupon.Id, userId
							);
					}

					TempData["CouponCode"] = code;
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine($"Failed to create coupon: {ex.Message}");
				}

				Session[CartSessionKey] = null;

				return RedirectToAction("Index", "Orders");
			}
		}
    }
}
