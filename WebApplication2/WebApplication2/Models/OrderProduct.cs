using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace WebApplication2.Models
{
    [Table("uzsakymas_produktas")] // Table name explicitly defined

    public class OrderProduct
    {
        public string table { get; set; }        
        public float cost { get; set; }
        public int ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; }

        public int OrderId { get; set; } // Foreign key to Warehouse
        public Order Order { get; set; }
    }
}