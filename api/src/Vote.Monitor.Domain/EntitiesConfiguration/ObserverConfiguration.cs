using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ObserverConfiguration : IEntityTypeConfiguration<Observer>
{
    public void Configure(EntityTypeBuilder<Observer> builder)
    {
        builder.ToTable("Observers");
        builder.HasOne(x => x.ApplicationUser);

        builder
            .HasMany(e => e.MonitoringObservers)
            .WithOne(e => e.Observer)
            .HasForeignKey(e => e.ObserverId)
            .IsRequired();
    }
}
