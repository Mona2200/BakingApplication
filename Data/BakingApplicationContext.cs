using BakingApplication.Data.Enitties;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace BakingApplication.Data;

public class BakingApplicationContext : DbContext
{
    public DbSet<Baking> Bakings { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<BakingOrder> BakingOrders { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<ExpenseType> ExpenseTypes { get; set; } = null!;

    public BakingApplicationContext()
    {
    }
    public BakingApplicationContext(DbContextOptions<BakingApplicationContext> options)
        : base(options)
    {
        Database.Migrate();
        //Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(ConfigurationManager.AppSettings["DbConnectString"]);
    }
}
