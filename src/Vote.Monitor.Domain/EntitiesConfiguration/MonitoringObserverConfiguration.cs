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
        builder.HasIndex(x => x.InviterNgoId);
    }
}
