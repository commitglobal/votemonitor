using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class CoalitionFormAccessConfiguration : IEntityTypeConfiguration<CoalitionFormAccess>
{
    public void Configure(EntityTypeBuilder<CoalitionFormAccess> builder)
    {
        builder
            .HasKey(m => new { m.CoalitionId, m.MonitoringNgoId , m.FormId}); // Composite primary key
        
        builder
            .HasOne(m => m.MonitoringNgo)
            .WithMany()
            .HasForeignKey(m => m.MonitoringNgoId);

        builder
            .HasOne(m => m.Coalition)
            .WithMany(c => c.FormAccess)
            .HasForeignKey(m => m.CoalitionId);
        
        builder
            .HasOne(m => m.Form)
            .WithMany()
            .HasForeignKey(m => m.FormId);
    }
}
