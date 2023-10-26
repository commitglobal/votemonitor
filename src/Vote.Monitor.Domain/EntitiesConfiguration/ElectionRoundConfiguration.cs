using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ElectionRoundConfiguration : IEntityTypeConfiguration<ElectionRound>
{
    public void Configure(EntityTypeBuilder<ElectionRound> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Status).IsRequired();
        builder.HasOne(e => e.Country).WithMany();
    }
}
