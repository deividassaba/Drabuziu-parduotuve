using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace WebApplication2.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DatabaseContext")
        {
            Database.SetInitializer<ApplicationDbContext>(null); // Disable automatic database creation
        }

        // Map models to existing database tables
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("vartotojas"); // Map to 'vartotojas' table
            modelBuilder.Entity<Administrators>().ToTable("administratorius"); // Map to 'administratorius' table
            modelBuilder.Entity<Buyers>().ToTable("pirkejas"); // Map to 'pirkejas' table
            modelBuilder.Entity<Sellers>().ToTable("pardavejas"); // Map to 'pardavejas' table
        }

        // DbSet properties for tables
        public DbSet<Users> Users { get; set; }
        public DbSet<Administrators> Administrators { get; set; }
        public DbSet<Buyers> Buyers { get; set; }
        public DbSet<Sellers> Sellers { get; set; }
    }

    // Users table mapping
    [Table("vartotojas")]
    public class Users
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("vardas")]
        [StringLength(255)]
        public string FirstName { get; set; }

        [Column("pavarde")]
        [StringLength(255)]
        public string LastName { get; set; }

        [Column("slaptazodis")]
        [StringLength(255)]
        public string Password { get; set; }

        [Column("telefononumeris")]
        [StringLength(255)]
        public string PhoneNumber { get; set; }
    }

    [Table("administratorius")]
    public class Administrators
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("arvadovas")]
        public Boolean Chief { get; set; }

        [Column("atlyginimas")]
        public double Salary { get; set; }

        [Column("kortelesnumeris")]
        [StringLength(255)]
        public string Card { get; set; }
    }


    // Buyers table mapping
    [Table("pirkejas")]
    public class Buyers
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("gimimodata")]
        public DateTime Birthday { get; set; }

        [Column("vieta")]
        [StringLength(255)]
        public string Place { get; set; }

        [Column("parduotuveskreditas")]
        public double Credit { get; set; }

        [Column("lytis")]
        public int Gender { get; set; }
    }

    // Sellers table mapping
    [Table("pardavejas")]
    public class Sellers
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("vieta")]
        [StringLength(255)]
        public string Place { get; set; }
    }
}
