using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.EmergencyAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class EmergencyConfiguration : IEntityTypeConfiguration<Emergency>
{
    public void Configure(EntityTypeBuilder<Emergency> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
      
        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId)
            .IsRequired();

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId)
            .IsRequired();       
        
        builder.HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId)
            .IsRequired(false);

        builder.HasMany(x => x.Attachments)
            .WithMany();

        builder.Property(x => x.PollingStationDescription).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(10_000).IsRequired();
        builder.Property(x => x.LocationType).HasMaxLength(10_000).IsRequired();
    }
}
