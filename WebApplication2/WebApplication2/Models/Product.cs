using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    [Table("produktai")] // Table name explicitly defined
    public class Product
    {
        public string table { get; set; }
        public int id { get; set; }

        public string name { get; set; }

        [StringLength(500)]
        public string description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal price { get; set; }

        // Stock Quantity
        [Required]
        [Range(0, int.MaxValue)]
        public int amount{ get; set; }

        public string imageURL{ get; set; }


        // Created Date (for auditing purposes)
        public DateTime CreatedDate { get; set; }
        //public Category[] Categories { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        public Product()
        {
            ProductCategories = new HashSet<ProductCategory>();
        }
    }
}