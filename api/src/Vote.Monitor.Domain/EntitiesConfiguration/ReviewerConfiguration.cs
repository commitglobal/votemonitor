using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ReviewerConfiguration : IEntityTypeConfiguration<Reviewer>
{
    public void Configure(EntityTypeBuilder<Reviewer> builder)
    {
        builder.ToTable(Tables.Reviewers);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.AssignedAt).IsRequired();

        builder.HasIndex(x => new { x.MonitoringNgoId, x.RecordType, x.RecordId, x.UserId }).IsUnique();
        builder.HasIndex(x => new { x.ElectionRoundId, x.UserId });

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
