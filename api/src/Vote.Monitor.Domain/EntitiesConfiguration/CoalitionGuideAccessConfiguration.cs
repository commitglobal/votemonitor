using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class CoalitionGuideAccessConfiguration : IEntityTypeConfiguration<CoalitionGuideAccess>
{
    public void Configure(EntityTypeBuilder<CoalitionGuideAccess> builder)
    {
        builder
            .HasKey(m => new { m.CoalitionId, m.MonitoringNgoId, m.GuideId }); // Composite primary key

        builder
            .HasOne(m => m.MonitoringNgo)
            .WithMany()
            .HasForeignKey(m => m.MonitoringNgoId);

        builder
            .HasOne(m => m.Coalition)
            .WithMany(c => c.GuideAccess)
            .HasForeignKey(m => m.CoalitionId);

        builder
            .HasOne(m => m.Guide)
            .WithMany()
            .HasForeignKey(m => m.GuideId);
    }
}
