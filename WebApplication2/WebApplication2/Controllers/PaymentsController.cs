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
using static Mysqlx.Expect.Open.Types.Condition.Types;

namespace WebApplication2.Controllers
{
    public class PaymentsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public float CalculateCOGSForCurrentMonth()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); 
            DateTime endDate = startDate.AddMonths(1).AddDays(-1); 

            var beginningInventory = db.Orders
                .Where(i => i.start < startDate)  
                .OrderByDescending(i => i.start)  
                .Select(i => i.itemCount)
                .FirstOrDefault(); 

            var totalPurchases = db.Payments
                .Join(db.Orders, p => p.FkUzsakymas, o => o.id, (p, o) => new { Payment = p, Order = o })
                .Where(x => x.Order.start >= startDate && x.Order.end <= endDate)  
                .Sum(x => (float?)x.Payment.cost) ?? 0;

            var endingInventory = db.Orders
            .Where(i => i.end <= endDate)
            .OrderByDescending(i => i.end)
            .Select(i => (float)i.itemCount) 
            .FirstOrDefault();

            var cogs = beginningInventory + totalPurchases - endingInventory;

            return cogs;
        }
        public ActionResult Index()
        {
            // Step 4: Calculate total revenue from the mokestis column
            float totalRevenue = 0;
            if (db.Payments.Count() > 0)
            {
                totalRevenue = db.Payments.Sum(m => m.cost);
            }
            // Step 5: Calculate gross profit (assume COGS logic exists)
            var totalCOGS = CalculateCOGSForCurrentMonth();
            var grossProfit = totalRevenue - totalCOGS;

            // Step 6: Calculate operating profit
            float? totalOperatingExpenses = 0;
            if (db.Payments.Count() > 0)
            {
                totalOperatingExpenses  = db.Salaries.Sum(e => e.cost); // Replace with more cost due to every warehouse
            }
            var operatingProfit = grossProfit - totalOperatingExpenses;

            // Step 7: Calculate profit margin
            var profitMargin = (operatingProfit / totalRevenue) * 100;

            // Step 8: Calculate total expenses
            var totalExpenses = totalOperatingExpenses + totalCOGS;

            // Step 9: Calculate net profit or loss

            // var otherIncome = db.OtherIncomes.Sum(o => o.Amount); // Replace with actual table

            var netProfit = totalRevenue - (totalExpenses + totalCOGS);
            
            var topClothes = db.Products
            .SelectMany(p => p.ProductCategories)  
            .GroupBy(c => c.Category)  
            .Select(g => new
            {
            Category = g.Key.name,  
            TotalSold = g.Count()  
            })
            .ToList();
            
            var monthlyRevenue = db.Payments
                       .Join(db.Orders, p => p.FkUzsakymas, o => o.id, (p, o) => new { p, o }) 
                       .GroupBy(po => po.o.start.Month) 
                       .Select(g => new
                       {
                           Month = g.Key,
                           Revenue = g.Sum(po => po.p.cost) 
                       })
                       .OrderBy(x => x.Month)
                       .ToList();

            ViewBag.TopClothesCategories = topClothes.Select(tc => tc.Category).ToList();
            ViewBag.TopClothesSold = topClothes.Select(tc => tc.TotalSold).ToList();


            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.GrossProfit = grossProfit;
            ViewBag.OperatingProfit = operatingProfit;
            ViewBag.ProfitMargin = profitMargin;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.NetProfit = netProfit;
            return View();
        }

    }
}
