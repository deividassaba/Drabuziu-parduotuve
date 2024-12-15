﻿using Microsoft.Build.Framework.XamlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Data;
using WebApplication2.Models;
using Category = WebApplication2.Models.Category;
namespace WebApplication2.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var categories = new List<Category>();
            /*foreach (var prct in product.ProductCategories)
            {
                categories.Add(db.Categories.Find(prct.CategoryId));
            }*/

            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            var categories = db.Categories.ToList(); // Assuming you have a Categories table
            ViewBag.Categories = categories;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,description,price,imageURL,manufacturer,mass")] Product product, 
         int[] categories)
        {
            if (ModelState.IsValid)
            {
                product.seller = null;
                db.Products.Add(product);
                db.SaveChanges();
                //product.Categories = new List<Category>();
                if (categories != null)
                {

                    foreach (var categoryId in categories)
                    {
                        
                        var categoryExists = db.Categories.Any(c => c.id == categoryId);
                        if (categoryExists)
                        {
                            System.Diagnostics.Debug.WriteLine($"Selected Category ID: {categoryId}");
                            db.Database.ExecuteSqlCommand(
                           "INSERT INTO kategorijaProduktas (fk_produktas, fk_kategorija) VALUES (@p0, @p1)",
                           parameters: new object[] { product.id, categoryId });
                            //product.Categories.Add(category);
                        }
                    }
                    //db.SaveChanges();
                }
                //return RedirectToAction("Index");
            }

            return RedirectToAction($"Details/{product.id}");
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            Product product = db.Products.Find(id);
            var categories = db.Categories.ToList(); // Assuming you have a Categories table
            ViewBag.Categories = categories;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,description,price,imageURL,manufacturer,mass")] Product product, int[] categories)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                foreach (var pc in db.ProductCategories.Where(p => p.ProductId == product.id).ToList())
                    db.ProductCategories.Remove(pc);
                db.SaveChanges();
                if (categories != null)
                {

                    foreach (var categoryId in categories)
                    {

                        var categoryExists = db.Categories.Any(c => c.id == categoryId);
                        if (categoryExists)
                        {
                            db.Database.ExecuteSqlCommand(
                           "INSERT INTO kategorijaProduktas (fk_produktas, fk_kategorija) VALUES (@p0, @p1)",
                           parameters: new object[] { product.id, categoryId });
                            //product.Categories.Add(category);
                        }
                    }
                    //db.SaveChanges();
                }
            }
            return RedirectToAction($"Details/{product.id}");
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            db.Products.Remove(product);
            /*db.ProductCategories.Where(p => p.ProductId == product.id).Remove()
            db.Database.ExecuteSqlCommand(
            "DELETE FROM kategorijaProduktas WHERE fk_produktas = @p0",
            parameters: new object[] { product.id });*/

            foreach (var pc in db.ProductCategories.Where(p => p.ProductId == product.id).ToList())
                db.ProductCategories.Remove(pc);
            db.SaveChanges();
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Shop()
        {
            var categories = db.Categories.ToList(); // Assuming you have a Categories table
            var products = db.Products.ToList();
            var manufacturers = new List<string>();
            foreach (var p in products)
            {
                if (!manufacturers.Contains(p.manufacturer))
                {
                    manufacturers.Add(p.manufacturer);
                }
            }
            ViewBag.manufacturers = manufacturers;
            ViewBag.Categories = categories;
            ViewBag.Products = products;
            ViewBag.filter_categories = new List<int>();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Shop(int[] filter_categories,
            string filter_text_name,
            string[] filter_values_manufacturer,
            string filter_text_manufacturer,
            float? filter_text_price_from,
            float? filter_text_price_to,
            int[] filter_values_price
            )
        {
            var categories = db.Categories.ToList();
            var catPr = db.ProductCategories.ToList();
            List<Product> products = db.Products.ToList();
            var manufacturers = new List<string>();

            //filters
            ViewBag.filter_categories = filter_categories != null ? (IEnumerable<int>)filter_categories : new List<int>();
            ViewBag.filter_text_name = filter_text_name != null ? filter_text_name : null;
            ViewBag.filter_values_manufacturer = filter_values_manufacturer != null ? (IEnumerable<string>)filter_values_manufacturer : new List<string>();
            ViewBag.filter_text_manufacturer = filter_text_manufacturer != null ? filter_text_manufacturer : null;
            ViewBag.filter_values_price = filter_values_price != null ? (IEnumerable<int>)filter_values_price : new List<int>();
            ViewBag.filter_text_price_from = filter_text_price_from != null ? filter_text_price_from : null;
            ViewBag.filter_text_price_to = filter_text_price_to != null ? filter_text_price_to : null;
            //

            foreach (var p in db.Products.ToList())
            {
                if (!manufacturers.Contains(p.manufacturer))
                {
                    manufacturers.Add(p.manufacturer);
                }
            }
            List<Product> products_temp;
            //category
            if (filter_categories != null)
            {
                products_temp = new List<Product>();
                foreach (var cp in catPr)
                {
                    foreach (var ca in filter_categories)
                    {
                        if (ca == cp.CategoryId)
                        {
                            products_temp.Add(db.Products.Find(cp.ProductId));
                        }
                    }
                }
                products = products_temp;
            }
            //name 

            if (filter_text_name != null && filter_text_name.Length > 0)
            {
                products_temp = new List<Product>();
                foreach (Product p in products)
                {
                    if (filter_text_name.Equals(p.name))
                    {
                        products_temp.Add(p);
                    }
                }
                products = products_temp;
            }

            //price
            if (true)
            {
                products_temp = new List<Product>();
                float min = -1;
                float max = 5000;
                min = filter_text_price_from != null ? (float)filter_text_price_from : min;
                max = filter_text_price_to != null ? (float)filter_text_price_to : max;

                foreach (Product p in products)
                {
                    if (p.price >= min && p.price <= max)
                    {
                        products_temp.Add(p);
                    }
                }
                products = products_temp;
            }
            //manufacturers
            if (filter_values_manufacturer != null)
            {
                products_temp = new List<Product>();
                foreach (Product p in products)
                {
                    if (filter_values_manufacturer.Contains(p.manufacturer))
                    {
                        products_temp.Add(p);
                    }
                }
                products = products_temp;
            }
            //variables
            ViewBag.Categories = categories;
            ViewBag.Products = products;
            ViewBag.manufacturers = manufacturers;
            //

            return View();
        }
    }

}
