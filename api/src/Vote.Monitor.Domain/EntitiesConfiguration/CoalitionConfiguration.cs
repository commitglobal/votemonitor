using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class CoalitionConfiguration : IEntityTypeConfiguration<Coalition>
{
    public void Configure(EntityTypeBuilder<Coalition> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.ElectionRoundId);

        builder.HasOne(x => x.ElectionRound).WithMany().HasForeignKey(e => e.ElectionRoundId);
        builder.HasOne(x => x.Leader).WithMany().HasForeignKey(e => e.LeaderId);
    }
}
