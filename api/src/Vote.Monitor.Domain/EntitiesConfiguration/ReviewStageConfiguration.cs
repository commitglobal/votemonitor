using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ReviewStageConfiguration : IEntityTypeConfiguration<ReviewStage>
{
    public void Configure(EntityTypeBuilder<ReviewStage> builder)
    {
        builder.ToTable(Tables.ReviewStages);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.Sequence).IsRequired();
        builder.Property(x => x.IsDefault).IsRequired();
        builder.Property(x => x.Color).HasMaxLength(32);
        builder.Property(x => x.Icon).HasMaxLength(32);

        // A sub-stage belongs to a stage of the same NGO; a stage with sub-stages can't be deleted (the API checks).
        builder.HasOne<ReviewStage>()
            .WithMany()
            .HasForeignKey(x => x.ParentStageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.MonitoringNgoId, x.Name }).IsUnique();

        // One default stage per monitoring NGO.
        builder.HasIndex(x => x.MonitoringNgoId)
            .IsUnique()
            .HasFilter("\"IsDefault\"")
            .HasDatabaseName("IX_ReviewStages_MonitoringNgoId_Default");
    }
}
