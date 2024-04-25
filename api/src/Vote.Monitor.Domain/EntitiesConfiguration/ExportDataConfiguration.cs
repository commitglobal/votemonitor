using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ExportedDataConfiguration : IEntityTypeConfiguration<ExportedData>
{
    public void Configure(EntityTypeBuilder<ExportedData> builder)
    {
        builder.ToTable(Tables.ExportedData);
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.FileName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.StartedAt).IsRequired();
        builder.Property(e => e.ExportStatus).IsRequired();
        builder.Property(e => e.Base64EncodedData);
        builder.Property(e => e.CompletedAt).IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.Ngo)
            .WithMany()
            .HasForeignKey(x => x.NgoId);
    }
}
