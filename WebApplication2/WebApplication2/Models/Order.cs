using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("uzsakymas")] // Table name explicitly defined

    public class Order
    {
        [Column("id")]
        public int id { get; set; }
        [Column("siuntimoadresas")]
        public string location { get; set; }
        [Column("kiekis")]
        public int itemCount { get; set; }
        [Column("verte")]
        public float cost { get; set; }
        [Column("pradzia")]
        public DateTime start { get; set; }
        [Column("pabaiga")]
        public DateTime end { get; set; }
        [Column("fk_pirkejas")]
        public int buyerId { get; set; }

        /*public virtual ICollection<OrderProduct> OrderProduct { get; set; }
        public Order()
        {
            OrderProduct = new HashSet<OrderProduct>();
        }*/
    }
}