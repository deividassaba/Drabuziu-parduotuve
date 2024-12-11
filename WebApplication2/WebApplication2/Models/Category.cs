using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("kategorijos")] // Table name explicitly defined

    public class Category
    {
        public string table{ get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int parentId { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        public Category()
        {
            ProductCategories = new HashSet<ProductCategory>();
        }
        /*public Category Parent { get; set; }
        public Product Products { get; set; }

        public Category[] getParents()
        {
            return new Category[0];
        }
        public Category[] getChildren()
        {
            return  new Category[0];
        }
        public Product[] getProducts()
        {
            return new Product[0];
        }*/
    }
}