using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.CountryAggregate;
using Vote.Monitor.Domain.Entities.CSOAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.EntitiesConfiguration;

namespace Vote.Monitor.Domain;

public class VoteMonitorContext : DbContext
{
    public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options) : base(options) { }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<CSO> CSOs { get; set; }
    public DbSet<PlatformAdmin> PlatformAdmins { get; set; }
    public DbSet<CSOAdmin> CSOAdmins { get; set; }
    public DbSet<Observer> Observers { get; set; }
    public DbSet<ElectionRound> ElectionRounds { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("uuid-ossp");

        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        builder.ApplyConfiguration(new CSOAdminConfiguration());
        builder.ApplyConfiguration(new ObserverConfiguration());
        builder.ApplyConfiguration(new PlatformAdminConfiguration());
        builder.ApplyConfiguration(new CountryConfiguration());
        builder.ApplyConfiguration(new CSOConfiguration());
        builder.ApplyConfiguration(new ElectionRoundConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }
}
