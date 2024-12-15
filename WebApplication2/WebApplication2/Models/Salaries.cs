using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("atlyginimas")]

    public class Salary
    {
        [Column("mokestis")]
        public float? cost { get; set; }

        [Column("bonusas")]
        public float? bonus { get; set; }

        [Key, Column(Order = 0)]
        public int FkAdministratorius { get; set; }

        [Key, Column(Order = 1)]
        public int FkAdministratorius1 { get; set; }
    }
}