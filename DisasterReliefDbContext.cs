using Microsoft.EntityFrameworkCore;
using disasterrelief_be.Models;

public class DisasterReliefDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Disaster> Disasters { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Category> Categories { get; set; }

    public DisasterReliefDbContext(DbContextOptions<DisasterReliefDbContext> context) : base(context)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }

}
