using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class RecordSeverityConfiguration : IEntityTypeConfiguration<RecordSeverity>
{
    public void Configure(EntityTypeBuilder<RecordSeverity> builder)
    {
        builder.ToTable(Tables.RecordSeverities);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.Severity).IsRequired();
        builder.Property(x => x.ChangedAt).IsRequired();

        builder.HasIndex(x => new { x.MonitoringNgoId, x.RecordType, x.RecordId }).IsUnique();

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
