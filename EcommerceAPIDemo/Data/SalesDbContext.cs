using EcommerceAPIDemo.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPIDemo.Data;

public class SalesDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<GameProduct> GameProducts { get; set; }
    public DbSet<GameCategory> GameCategories { get; set; }
    public DbSet<Sale> Sales { get; set; }

    public DbSet<GameProductCategory> ProductCategories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameProductCategory>()
            .HasKey(key => new { key.GameProductId, key.GameCategoryId });

        modelBuilder.Entity<GameProductCategory>()
            .HasOne(productCategory => productCategory.GameProduct)
            .WithMany(product => product.ProductCategories)
            .HasForeignKey(productCategory => productCategory.GameProductId);

        modelBuilder.Entity<GameProductCategory>()
            .HasOne(productCategory => productCategory.GameCategory)
            .WithMany(category => category.ProductCategories)
            .HasForeignKey(productCategory => productCategory.GameCategoryId);
    }
}
