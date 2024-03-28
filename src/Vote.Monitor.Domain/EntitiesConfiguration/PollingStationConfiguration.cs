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
        builder.Property(p => p.Tags).IsRequired(false);
        builder.Property(p => p.Level1).HasMaxLength(256).IsRequired();
        builder.Property(p => p.Level2).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Level3).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Level4).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Level5).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Number).HasMaxLength(256).IsRequired();

        builder
            .HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
