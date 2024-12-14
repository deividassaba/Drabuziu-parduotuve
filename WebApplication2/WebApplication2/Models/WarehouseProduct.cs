using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("sandelis_produktas")] // Table name explicitly defined

    public class WarehouseProduct
    {
        public string table { get; set; }
        public int ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; }

        public int WarehouseId { get; set; } // Foreign key to Warehouse
        public Warehouse Warehouse { get; set; }
    }
}