using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("kategorijaProduktas")] // Table name explicitly defined

    public class ProductCategory
    {
        [Column("fk_produktas")]
        public int ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; }

        [Column("fk_kategorija")]
        public int CategoryId { get; set; } // Foreign key to Category
        public Category Category { get; set; }
    }
}