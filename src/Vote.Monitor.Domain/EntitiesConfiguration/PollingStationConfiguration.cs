using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class PollingStationConfiguration : IEntityTypeConfiguration<PollingStation>
{
    public void Configure(EntityTypeBuilder<PollingStation> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.Address).HasMaxLength(2024).IsRequired();
        builder.Property(p => p.DisplayOrder).IsRequired();
        builder.Property(p => p.Tags).IsRequired();
    }
}
