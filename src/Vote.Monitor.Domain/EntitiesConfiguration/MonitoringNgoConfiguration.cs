using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class MonitoringNgoConfiguration : IEntityTypeConfiguration<MonitoringNgo>
{
    public void Configure(EntityTypeBuilder<MonitoringNgo> builder)
    {
        builder.ToTable("MonitoringNgo");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();

        builder.Property(e => e.Status).IsRequired();

        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.NgoId);
        
        builder
            .HasMany(x => x.MonitoringObservers)
            .WithOne(x => x.InviterNgo)
            .HasForeignKey(x => x.InviterNgoId);

        builder.Navigation(nameof(MonitoringNgo.MonitoringObservers))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
