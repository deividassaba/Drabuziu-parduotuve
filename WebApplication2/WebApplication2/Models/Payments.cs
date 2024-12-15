using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("Mokestis")] // Table name explicitly defined

    public class Payment
    {
        [Column("id")]
        public int id { get; set; }
        [Column("mokestis")]
        public string count { get; set; }
    }
}