using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class IssueReportConfiguration : IEntityTypeConfiguration<IssueReport>
{
    public void Configure(EntityTypeBuilder<IssueReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FollowUpStatus)
            .IsRequired()
            .HasDefaultValue(IssueReportFollowUpStatus.NotApplicable);

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);

        builder.Property(x => x.NumberOfQuestionsAnswered);
        builder.Property(x => x.NumberOfFlaggedAnswers);

        builder.Property(e => e.LocationType).IsRequired();
        
        builder
            .Property(e => e.LocationDescription)
            .IsRequired(false)
            .HasMaxLength(1024);
        
        builder
            .HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId)
            .IsRequired(false);
        
        builder.Property(x => x.Answers)
            .HasConversion<AnswersToJsonConverter, AnswersValueComparer>()
            .HasColumnType("jsonb");
        
        builder
            .HasMany(x => x.Notes)
            .WithOne(x => x.IssueReport)
            .HasForeignKey(x => x.IssueReportId);

        builder
            .HasMany(x => x.Attachments)
            .WithOne(x => x.IssueReport)
            .HasForeignKey(x => x.IssueReportId);

        builder.Navigation(nameof(IssueReport.Notes))
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(nameof(IssueReport.Attachments))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}