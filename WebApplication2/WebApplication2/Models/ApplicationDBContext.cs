using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DatabaseContext")
        {
            Database.SetInitializer<ApplicationDbContext>(null); // Disable automatic database creation
        }


        // Disable the creation of a new Users table
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("vartotojas"); // Specify the existing table name
        }

        // This should map directly to the existing 'vartotojas' table
        public DbSet<User> Users { get; set; }
    }

    [Table("vartotojas")] // Directly map to the existing 'vartotojas' table
    public class User
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
}
