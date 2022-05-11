using Bogus;
using Microsoft.EntityFrameworkCore;

namespace MiniProfilerSite.Models;

public class Database : DbContext
{
    public DbSet<Product> Products { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            
            .UseSqlite("Data Source=database.db");
    }

    
}

public class Product 
{
    public int Id { get; set; }
    public double Price { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public static class DatabaseInitialization {
    public static void InitializeDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Database>();
        
        // migrate the database to latest
        db.Database.Migrate();
        
        var containsProducts = db.Products.Any();
        if (containsProducts) return;

        var count = 1;
        var products = new Faker<Product>()
            .RuleFor(m => m.Id, m => count++)
            .RuleFor(m => m.Category, (f, p) => f.Commerce.Categories(1).First())
            .RuleFor(m => m.Department, (f, p) => f.Commerce.Department(1))
            .RuleFor(m => m.Name, (f, p) => f.Commerce.ProductName())
            .RuleFor(m => m.Price, (f, p) => (double)f.Finance.Amount())
            .RuleFor(m => m.CompanyName, (f, p) => f.Company.CompanyName());
        
        // yes, will be slow
        db.Products.AddRange(products.Generate(10_000));
        db.SaveChanges();
    }
}
