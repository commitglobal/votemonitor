using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CoalitionAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class CoalitionMembershipConfiguration : IEntityTypeConfiguration<CoalitionMembership>
{
    public void Configure(EntityTypeBuilder<CoalitionMembership> builder)
    {
        builder
            .HasKey(m => new { m.MonitoringNgoId, m.CoalitionId }); // Composite primary key

        builder.HasIndex(m => new { m.MonitoringNgoId, m.CoalitionId, m.ElectionRoundId }).IsUnique();
        builder.HasIndex(m => new { m.MonitoringNgoId, m.ElectionRoundId }).IsUnique(); // only one membership per election

        builder
            .HasOne(m => m.MonitoringNgo)
            .WithMany(mn => mn.Memberships)
            .HasForeignKey(m => m.MonitoringNgoId);

        builder
            .HasOne(m => m.Coalition)
            .WithMany(c => c.Memberships)
            .HasForeignKey(m => m.CoalitionId);

        builder.HasOne(x => x.ElectionRound).WithMany().HasForeignKey(e => e.ElectionRoundId);
    }
}
