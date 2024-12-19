using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecretSantaBots.DataBase.Models;

namespace SecretSantaBots.DataBase;

public class ApplicationDbContext:DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Participant> Participants { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Game>()
            .HasMany(g => g.Participants)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId);

        modelBuilder.Entity<Participant>()
            .HasOne(p => p.AssignedTo)
            .WithOne()
            .HasForeignKey<Participant>(p => p.AssignedToId);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) // Проверка, чтобы не переопределять уже настроенные параметры
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=SantaBogema;Username=postgres;password=123456");
        }
    }
   public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));
            });
}