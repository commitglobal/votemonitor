using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain;

public class VoteMonitorContext : DbContext
{
    private readonly ISerializerService _serializerService;
    private readonly ITimeProvider _timeProvider;
    private readonly ICurrentUserProvider _currentUserProvider;

    public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options,
        ISerializerService serializerService,
        ITimeProvider timeProvider,
        ICurrentUserProvider currentUserProvider) : base(options)
    {
        _serializerService = serializerService;
        _timeProvider = timeProvider;
        _currentUserProvider = currentUserProvider;
    }

    public DbSet<ApplicationUser> Users { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Ngo> Ngos { get; set; }
    public DbSet<PlatformAdmin> PlatformAdmins { get; set; }
    public DbSet<NgoAdmin> NgoAdmins { get; set; }
    public DbSet<Observer> Observers { get; set; }
    public DbSet<PollingStation> PollingStations { get; set; }
    public DbSet<ElectionRound> ElectionRounds { get; set; }
    public DbSet<ImportValidationErrors> ImportValidationErrors { set; get; }
    public DbSet<Trail> AuditTrails => Set<Trail>();
    public DbSet<FormTemplate> FormTemplates { set; get; }

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
        builder.ApplyConfiguration(new NgoAdminConfiguration());
        builder.ApplyConfiguration(new ObserverConfiguration());
        builder.ApplyConfiguration(new PlatformAdminConfiguration());
        builder.ApplyConfiguration(new CountryConfiguration());
        builder.ApplyConfiguration(new LanguageConfiguration());
        builder.ApplyConfiguration(new NgoConfiguration());
        builder.ApplyConfiguration(new ElectionRoundConfiguration());
        builder.ApplyConfiguration(new MonitoringNgoConfiguration());
        builder.ApplyConfiguration(new MonitoringObserverConfiguration());
        builder.ApplyConfiguration(new PollingStationConfiguration());
        builder.ApplyConfiguration(new ImportValidationErrorsConfiguration());
        builder.ApplyConfiguration(new FormTemplateConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new NotificationTokenConfiguration());
        builder.ApplyConfiguration(new PollingStationAttachmentConfiguration());
        builder.ApplyConfiguration(new PollingStationNoteConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditingInterceptor(_currentUserProvider, _timeProvider));
        optionsBuilder.AddInterceptors(new AuditTrailInterceptor(_serializerService, _currentUserProvider, _timeProvider));
    }
}
