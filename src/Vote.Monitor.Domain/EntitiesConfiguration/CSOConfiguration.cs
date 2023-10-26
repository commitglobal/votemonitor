using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CSOAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CSOConfiguration : IEntityTypeConfiguration<CSO>
{
    public void Configure(EntityTypeBuilder<CSO> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Status).IsRequired();
    }
}
