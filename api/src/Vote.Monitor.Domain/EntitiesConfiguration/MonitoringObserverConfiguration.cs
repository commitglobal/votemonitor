using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class MonitoringObserverConfiguration : IEntityTypeConfiguration<MonitoringObserver>
{
    public void Configure(EntityTypeBuilder<MonitoringObserver> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.Tags).IsRequired();
        builder.HasIndex(x => new { x.ElectionRoundId, x.Id }).IsUnique();

        builder
            .HasOne(e => e.ElectionRound)
            .WithMany()
            .HasForeignKey(e => e.ElectionRoundId)
            .IsRequired();

        builder
            .HasOne(e => e.MonitoringNgo)
            .WithMany(e => e.MonitoringObservers)
            .HasForeignKey(e => e.MonitoringNgoId)
            .IsRequired();

        builder
            .HasOne(e => e.Observer)
            .WithMany(e => e.MonitoringObservers)
            .HasForeignKey(e => e.ObserverId)
            .IsRequired();
    }
}
