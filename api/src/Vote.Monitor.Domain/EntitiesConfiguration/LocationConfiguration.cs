using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.LocationAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.DisplayOrder).IsRequired();
        builder.Property(p => p.Tags).IsRequired(false);
        builder.Property(p => p.Level1).HasMaxLength(256).IsRequired();
        builder.Property(p => p.Level2).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Level3).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Level4).HasMaxLength(256).IsRequired(false);
        builder.Property(p => p.Level5).HasMaxLength(256).IsRequired(false);

        builder
            .HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}