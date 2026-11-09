using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.RecordLabelAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class RecordLabelConfiguration : IEntityTypeConfiguration<RecordLabel>
{
    public void Configure(EntityTypeBuilder<RecordLabel> builder)
    {
        builder.ToTable(Tables.RecordLabels);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(RecordLabel.MaxLength).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        // A record's labels are a set, compared case-insensitively: the unique index on
        // ("MonitoringNgoId", "RecordType", "RecordId", lower("Label")) is created in the AddRecordLabels migration,
        // as EF can't model an expression index. This one lists the NGO's labels in a round.
        builder.HasIndex(x => new { x.ElectionRoundId, x.MonitoringNgoId });

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
