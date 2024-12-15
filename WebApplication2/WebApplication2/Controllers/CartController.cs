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

        // GET: Cart
        public ActionResult Index()
        {
            var cart = Session[CartSessionKey] as List<CartItem> ?? new List<CartItem>();
            return View(cart);
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
