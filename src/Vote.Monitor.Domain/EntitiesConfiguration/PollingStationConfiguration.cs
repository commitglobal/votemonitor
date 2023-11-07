using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class PollingStationConfiguration : IEntityTypeConfiguration<PollingStation>
{
    public void Configure(EntityTypeBuilder<PollingStation> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Address).IsRequired();
        builder.Property(e => e.DisplayOrder).IsRequired();
        builder.Property(e => e.Tags).IsRequired();
    }
}
