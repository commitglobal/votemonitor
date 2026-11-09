using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.FollowUpAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class FollowUpStatusDefinitionConfiguration : IEntityTypeConfiguration<FollowUpStatusDefinition>
{
    public void Configure(EntityTypeBuilder<FollowUpStatusDefinition> builder)
    {
        builder.ToTable(Tables.FollowUpStatuses);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(32);
        builder.Property(x => x.Icon).HasMaxLength(32);
        builder.Property(x => x.LegacyStatus).HasMaxLength(32);

        builder.HasIndex(x => new { x.MonitoringNgoId, x.Name }).IsUnique();

        // One default status per monitoring NGO.
        builder.HasIndex(x => x.MonitoringNgoId)
            .IsUnique()
            .HasFilter("\"IsDefault\"")
            .HasDatabaseName("IX_FollowUpStatuses_MonitoringNgoId_Default");
    }
}

internal class RecordFollowUpConfiguration : IEntityTypeConfiguration<RecordFollowUp>
{
    public void Configure(EntityTypeBuilder<RecordFollowUp> builder)
    {
        builder.ToTable(Tables.RecordFollowUps);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.ChangedAt).IsRequired();

        builder.HasIndex(x => new { x.MonitoringNgoId, x.RecordType, x.RecordId }).IsUnique();
        builder.HasIndex(x => x.FollowUpStatusId);

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);

        // A status still in use can't be deleted; its reports are moved first.
        builder.HasOne(x => x.FollowUpStatus)
            .WithMany()
            .HasForeignKey(x => x.FollowUpStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
