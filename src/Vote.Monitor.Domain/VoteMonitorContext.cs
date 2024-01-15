using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SmartEnum.EFCore;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Core.Services.Serialization;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
using Vote.Monitor.Domain.Entities.Auditing;
using Vote.Monitor.Domain.Entities.CountryAggregate;
using Vote.Monitor.Domain.Entities.CSOAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;
using Vote.Monitor.Domain.EntitiesConfiguration;

namespace Vote.Monitor.Domain;

public class VoteMonitorContext : DbContext
{
    private readonly ISerializerService _serializerService;
    private readonly ITimeService _timeService;
    private readonly ICurrentUser _currentUser;

    public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options,
        ISerializerService serializerService,
        ITimeService timeService,
        ICurrentUser currentUser) : base(options)
    {
        _serializerService = serializerService;
        _timeService = timeService;
        _currentUser = currentUser;
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<CSO> CSOs { get; set; }
    public DbSet<PlatformAdmin> PlatformAdmins { get; set; }
    public DbSet<CSOAdmin> CSOAdmins { get; set; }
    public DbSet<Observer> Observers { get; set; }
    public DbSet<PollingStation> PollingStations { get; set; }

    public DbSet<ElectionRound> ElectionRounds { get; set; }
    public DbSet<Trail> AuditTrails => Set<Trail>();

    public DbSet<ImportValidationErrors> ImportValidationErrors { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("uuid-ossp");

        var method = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ObjectKeys), new[] { typeof(JsonDocument) });

        builder
            .HasDbFunction(method!)
            .HasName("jsonb_object_keys");

        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        builder.ApplyConfiguration(new CSOAdminConfiguration());
        builder.ApplyConfiguration(new ObserverConfiguration());
        builder.ApplyConfiguration(new PlatformAdminConfiguration());
        builder.ApplyConfiguration(new CountryConfiguration());
        builder.ApplyConfiguration(new LanguageConfiguration());
        builder.ApplyConfiguration(new CSOConfiguration());
        builder.ApplyConfiguration(new ElectionRoundConfiguration());
        builder.ApplyConfiguration(new PollingStationConfiguration());
        builder.ApplyConfiguration(new ImportValidationErrorsConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var auditEntries = HandleAuditingBeforeSaveChanges(_currentUser.GetUserId());

        int result = await base.SaveChangesAsync(cancellationToken);

        await HandleAuditingAfterSaveChangesAsync(auditEntries, cancellationToken);

        return result;
    }

    private List<AuditTrail> HandleAuditingBeforeSaveChanges(Guid userId)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.LastModifiedBy = userId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = userId;
                    break;
            }
        }

        ChangeTracker.DetectChanges();

        var trailEntries = new List<AuditTrail>();
        foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .ToList())
        {
            var trailEntry = new AuditTrail(entry, _serializerService, _timeService)
            {
                TableName = entry.Entity.GetType().Name,
                UserId = userId
            };
            trailEntries.Add(trailEntry);
            foreach (var property in entry.Properties)
            {
                if (property.IsTemporary)
                {
                    trailEntry.TemporaryProperties.Add(property);
                    continue;
                }

                string propertyName = property.Metadata.Name;
                if (property.Metadata.IsPrimaryKey())
                {
                    trailEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        trailEntry.TrailType = TrailType.Create;
                        trailEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        trailEntry.TrailType = TrailType.Delete;
                        trailEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified && property.OriginalValue?.Equals(property.CurrentValue) == false)
                        {
                            trailEntry.ChangedColumns.Add(propertyName);
                            trailEntry.TrailType = TrailType.Update;
                            trailEntry.OldValues[propertyName] = property.OriginalValue;
                            trailEntry.NewValues[propertyName] = property.CurrentValue;
                        }

                        break;
                }
            }
        }

        foreach (var auditEntry in trailEntries.Where(e => !e.HasTemporaryProperties))
        {
            AuditTrails.Add(auditEntry.ToAuditTrail());
        }

        return trailEntries.Where(e => e.HasTemporaryProperties).ToList();
    }

    private Task HandleAuditingAfterSaveChangesAsync(List<AuditTrail> trailEntries, CancellationToken cancellationToken = new())
    {
        if (trailEntries == null || trailEntries.Count == 0)
        {
            return Task.CompletedTask;
        }

        foreach (var entry in trailEntries)
        {
            foreach (var prop in entry.TemporaryProperties)
            {
                if (prop.Metadata.IsPrimaryKey())
                {
                    entry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                }
                else
                {
                    entry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            AuditTrails.Add(entry.ToAuditTrail());
        }

        return SaveChangesAsync(cancellationToken);
    }

}
