using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class PollingStationCorrectionConfiguration : IEntityTypeConfiguration<PollingStationCorrection>
{
    public void Configure(EntityTypeBuilder<PollingStationCorrection> builder)
    {
        builder.ToTable(Tables.PollingStationCorrections);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => new { x.ElectionRoundId, x.RecordType, x.RecordId });

        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.CorrectedAt).IsRequired();
        builder.Property(x => x.CorrectedBy).IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.FromPollingStation)
            .WithMany()
            .HasForeignKey(x => x.FromPollingStationId);

        builder.HasOne(x => x.ToPollingStation)
            .WithMany()
            .HasForeignKey(x => x.ToPollingStationId);
    }
}
