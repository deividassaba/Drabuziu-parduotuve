using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    [Table("nuolaidoskodas")] // Table name explicitly defined

    public class Coupon
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("sukurimodata")]
        public Nullable<DateTime> Sukurimo_data { get; set; }

        [Column("veikimopradziosdata")]
        public Nullable<DateTime> Veikimo_pradzios_data { get; set; }

        [Column("panaudojimuskaičius")]
        public Nullable<int> Panaudojimu_sk { get; set; }

        [Column("kodas")]
        [Required]
        public string Kodas { get; set; }

        [Column("verte")]
        [Required]
        public float Verte { get; set; }

        [Column("aprasymas")]
		public Nullable<string> Aprasymas { get; set; }

        [Column("pavadinimas")]
        [Required]
        public string Pavadinimas { get; set; }

        [Column("galiojimopabaigosdata")]
        public Nullable<DateTime> Galiojimo_pabaigos_data { get; set; }

        [Column("yraribotas")]
        public bool Yra_ribotas { get; set; }

    }
}
