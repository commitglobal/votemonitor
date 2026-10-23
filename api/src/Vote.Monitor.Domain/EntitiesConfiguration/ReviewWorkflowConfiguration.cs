using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ReviewWorkflowConfiguration : IEntityTypeConfiguration<ReviewWorkflow>
{
    public void Configure(EntityTypeBuilder<ReviewWorkflow> builder)
    {
        builder.ToTable(Tables.ReviewWorkflows);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.StageChangedAt).IsRequired();

        builder.HasIndex(x => new { x.MonitoringNgoId, x.RecordType, x.RecordId }).IsUnique();
        builder.HasIndex(x => x.StageId);

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);

        // A stage still in use can't be deleted; its reports are moved first.
        builder.HasOne(x => x.Stage)
            .WithMany()
            .HasForeignKey(x => x.StageId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
