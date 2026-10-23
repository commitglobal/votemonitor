using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ReviewActivityConfiguration : IEntityTypeConfiguration<ReviewActivity>
{
    public void Configure(EntityTypeBuilder<ReviewActivity> builder)
    {
        builder.ToTable(Tables.ReviewActivities);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.Kind).IsRequired();
        builder.Property(x => x.ActorName).HasMaxLength(256);
        builder.Property(x => x.RuleName).HasMaxLength(256);
        builder.Property(x => x.OccurredAt).IsRequired();
        builder.Property(x => x.FromStageName).HasMaxLength(64);
        builder.Property(x => x.ToStageName).HasMaxLength(64);
        builder.Property(x => x.ReviewerName).HasMaxLength(256);
        builder.Property(x => x.FollowUpStatus).HasMaxLength(64);
        builder.Property(x => x.FromSeverity);
        builder.Property(x => x.ToSeverity);

        builder.HasIndex(x => new { x.MonitoringNgoId, x.RecordType, x.RecordId, x.OccurredAt });

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
