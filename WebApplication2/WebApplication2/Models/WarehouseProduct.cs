using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("sandelisproduktas")] // Table name explicitly defined

    public class WarehouseProduct
    {
        [Column("fk_produktas")]
        public int ProductId { get; set; } // Foreign key to Product
        public Product Product { get; set; }
        [Column("fk_sandelis")]
        public int WarehouseId { get; set; } // Foreign key to Warehouse
        public Warehouse Warehouse { get; set; }
        [Column("kiekis")]
        public int amount { get; set; }
    }
}