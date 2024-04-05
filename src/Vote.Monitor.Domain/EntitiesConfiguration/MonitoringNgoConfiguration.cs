using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class MonitoringNgoConfiguration : IEntityTypeConfiguration<MonitoringNgo>
{
    public void Configure(EntityTypeBuilder<MonitoringNgo> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();

        builder.Property(e => e.Status).IsRequired();

        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.NgoId);

        builder
            .HasMany(e => e.MonitoringObservers)
            .WithOne(e => e.MonitoringNgo)
            .HasForeignKey(e => e.MonitoringNgoId)
            .IsRequired();

        builder.Navigation(nameof(MonitoringNgo.MonitoringObservers))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
