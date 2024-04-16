using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class NgoConfiguration : IEntityTypeConfiguration<Ngo>
{
    public void Configure(EntityTypeBuilder<Ngo> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
        builder.Property(c => c.Status).IsRequired();
    }
}
