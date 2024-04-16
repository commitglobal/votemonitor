﻿using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.NgoAdminAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;
using Vote.Monitor.Domain.Entities.NotificationAggregate;
using Vote.Monitor.Domain.Entities.NotificationStubAggregate;
using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAttachmentAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

namespace Vote.Monitor.Domain;

public class VoteMonitorContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly ISerializerService _serializerService;
    private readonly ITimeProvider _timeProvider;
    private readonly ICurrentUserIdProvider _currentUserIdProvider;
    public VoteMonitorContext(DbContextOptions<VoteMonitorContext> options,
        ISerializerService serializerService,
        ITimeProvider timeProvider,
        ICurrentUserIdProvider currentUserIdProvider) : base(options)
    {
        _serializerService = serializerService;
        _timeProvider = timeProvider;
        _currentUserIdProvider = currentUserIdProvider;
    }

    // Used by Dapper
    public IDbConnection Connection => Database.GetDbConnection();
    public DbSet<Country> Countries { get; set; }
    public DbSet<Ngo> Ngos { get; set; }
    public DbSet<NgoAdmin> NgoAdmins { get; set; }
    public DbSet<Observer> Observers { get; set; }
    public DbSet<PollingStation> PollingStations { get; set; }
    public DbSet<ElectionRound> ElectionRounds { get; set; }
    public DbSet<ImportValidationErrors> ImportValidationErrors { set; get; }
    public DbSet<Trail> AuditTrails => Set<Trail>();
    public DbSet<FormTemplate> FormTemplates { set; get; }
    public DbSet<Form> Forms { set; get; }
    public DbSet<FormSubmission> FormSubmissions { set; get; }
    public DbSet<PollingStationInformationForm> PollingStationInformationForms { set; get; }
    public DbSet<PollingStationInformation> PollingStationInformation { set; get; }
    public DbSet<MonitoringNgo> MonitoringNgos { set; get; }
    public DbSet<MonitoringObserver> MonitoringObservers { set; get; }
    public DbSet<ObserverGuide> ObserversGuides { set; get; }
    public DbSet<NotificationToken> NotificationTokens { set; get; }
    public DbSet<Notification> Notifications { set; get; }
    public DbSet<PollingStationAttachment> PollingStationAttachments { set; get; }
    public DbSet<PollingStationNote> PollingStationNotes { set; get; }
    public DbSet<NotificationStub> NotificationStubs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasPostgresExtension("uuid-ossp");

        var jsonbObjectKeys = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ObjectKeys), new[] { typeof(JsonDocument) });

        var unnest = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.Unnest), new[] { typeof(string[]) });

        var arrayUnique = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ArrayUnique), new[] { typeof(string[]) });

        var arrayRemove = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ArrayRemove), new[] { typeof(string[]), typeof(string) });

        var arrayDiff = typeof(Postgres.Functions)
            .GetRuntimeMethod(nameof(Postgres.Functions.ArrayDiff), new[] { typeof(string[]), typeof(string[]) });

        builder
            .HasDbFunction(jsonbObjectKeys!)
            .HasName("jsonb_object_keys");

        builder
            .HasDbFunction(unnest!)
            .HasName("unnest");

        builder
            .HasDbFunction(arrayUnique!)
            .HasName(CustomDBFunctions.ArrayUnique);

        builder
            .HasDbFunction(arrayRemove!)
            .HasName("array_remove");

        builder
            .HasDbFunction(arrayDiff!)
            .HasName(CustomDBFunctions.ArrayDiff);

        builder.ApplyConfiguration(new ApplicationUserConfiguration());
        builder.ApplyConfiguration(new NgoAdminConfiguration());
        builder.ApplyConfiguration(new ObserverConfiguration());
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
        builder.ApplyConfiguration(new PollingStationInformationFormConfiguration());
        builder.ApplyConfiguration(new PollingStationInformationConfiguration());
        builder.ApplyConfiguration(new ObserverGuideConfiguration());
        builder.ApplyConfiguration(new FormConfiguration());
        builder.ApplyConfiguration(new FormSubmissionConfiguration());
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new NotificationStubConfiguration());
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.ConfigureSmartEnum();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditingInterceptor(_currentUserIdProvider, _timeProvider));
        optionsBuilder.AddInterceptors(new AuditTrailInterceptor(_serializerService, _currentUserIdProvider, _timeProvider));
    }
}
