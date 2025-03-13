using EntityDal.Entity;
using EntityDal.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace EntityDal;

public class MainContext : DbContext
{
    public DbSet<TelegramUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = "Data Source=WIN-VS038T41FAP;User ID=sa;Password=2;Initial Catalog=MyBot;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
