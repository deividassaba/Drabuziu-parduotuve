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

namespace WebApplication2.Controllers
{
    public class CouponsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Coupons
        public ActionResult Index()
        {
            return View(db.Coupons.ToList());
        }

        // GET: Coupons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        // GET: Coupons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Coupons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Coupon coupon)
		{
			if (!ModelState.IsValid)
				return View(coupon);

			try 
			{
				coupon.Sukurimo_data = DateTime.UtcNow;
                // Keep generating codes until we find a unique one
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
				return RedirectToAction("Index");
			}
			catch (Exception ex)
			{
				var fullError = "";
				var currentEx = ex;
				while (currentEx != null)
				{
					fullError += currentEx.Message + " | ";
					currentEx = currentEx.InnerException;
				}
				Console.WriteLine("Full error: " + fullError);
				ModelState.AddModelError("", fullError);
				return View(coupon);
			}
		}

        // GET: Coupons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Sukurimo_data,Veikimo_pradzios_data,Panaudojimu_sk,Kodas,Verte,Aprasymas,Pavadinimas,Galiojimo_pabaigos_data,Yra_ribotas")] Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(coupon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(coupon);
        }

        // GET: Coupons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Coupon coupon = db.Coupons.Find(id);
            if (coupon == null)
            {
                return HttpNotFound();
            }
            return View(coupon);
        }

        // POST: Coupons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Coupon coupon = db.Coupons.Find(id);
            db.Coupons.Remove(coupon);
            db.SaveChanges();
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
    }
}
