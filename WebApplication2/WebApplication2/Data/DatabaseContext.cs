using System.Data.Entity;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("name=DatabaseContext")
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        //public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*
            modelBuilder.Entity<Product>()
            .HasMany(p => p.Categories)
            .WithMany(c => c.Products)
            .Map(m =>
            {
                m.ToTable("ProductCategory");
                m.MapLeftKey("ProductId");
                m.MapRightKey("CategoryId");
            });
            */
            base.OnModelCreating(modelBuilder);
            /*
            // Configure the many-to-many relationship using the ProductCategory join table
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId }); // Composite key

            modelBuilder.Entity<ProductCategory>()
                .HasRequired(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasRequired(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);*/
        }

        public System.Data.Entity.DbSet<WebApplication2.Models.Order> Orders { get; set; }
        public System.Data.Entity.DbSet<WebApplication2.Models.Warehouse> Warehouses { get; set; }
    }
}
