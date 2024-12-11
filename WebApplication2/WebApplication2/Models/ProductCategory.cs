using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("produktas_kategorija")] // Table name explicitly defined

    public class ProductCategory
    {
        public string table { get; set; }
        public int ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; }

        public int CategoryId { get; set; } // Foreign key to Category
        public Category Category { get; set; }
    }
}