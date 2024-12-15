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

            // Create a new order
            var order = new Order
            {
                location = "Customer's Address", // Replace with actual data
                itemCount = cart.Sum(c => c.Quantity),
                cost = (float)cart.Sum(c => c.Price * c.Quantity),
                start = DateTime.Now,
                end = DateTime.Now.AddDays(3), // Example delivery time
                status = "Pateiktas",
                buyerId = 1, // Replace with actual buyer ID
                OrderProduct = cart.Select(c => new OrderProduct
                {
                    ProductId = c.ProductId,
                    cost = (float)(c.Price * c.Quantity)
                }).ToList()
            };

            using (var db = new DatabaseContext())
            {
                db.Orders.Add(order);
                db.SaveChanges();
            }

            // Clear cart after checkout
            Session[CartSessionKey] = null;

            return RedirectToAction("Index", "Orders");
        }
    }
}
