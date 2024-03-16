using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.PollingStationNoteAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class PollingStationNoteConfiguration : IEntityTypeConfiguration<PollingStationNote>
{
    public void Configure(EntityTypeBuilder<PollingStationNote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.MonitoringObserverId);

        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();

        builder.Property(x => x.Timestamp);

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);
    }
}
