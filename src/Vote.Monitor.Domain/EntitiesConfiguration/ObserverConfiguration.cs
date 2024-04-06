using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ObserverConfiguration : IEntityTypeConfiguration<Observer>
{
    public void Configure(EntityTypeBuilder<Observer> builder)
    {
        builder.ToTable("Observers");
        builder.Property(x => x.PhoneNumber).HasMaxLength(32);

        builder
            .HasMany(e => e.MonitoringObservers)
            .WithOne(e => e.Observer)
            .HasForeignKey(e => e.ObserverId)
            .IsRequired();
    }
}
