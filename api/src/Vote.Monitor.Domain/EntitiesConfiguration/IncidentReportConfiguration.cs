using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class IncidentReportConfiguration : IEntityTypeConfiguration<IncidentReport>
{
    public void Configure(EntityTypeBuilder<IncidentReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FollowUpStatus)
            .IsRequired()
            .HasDefaultValue(IncidentReportFollowUpStatus.NotApplicable);

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
        builder.Property(x => x.LastUpdatedAt).IsRequired();

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
            .WithOne(x => x.IncidentReport)
            .HasForeignKey(x => x.IncidentReportId);

        builder
            .HasMany(x => x.Attachments)
            .WithOne(x => x.IncidentReport)
            .HasForeignKey(x => x.IncidentReportId);

        builder.Navigation(nameof(IncidentReport.Notes))
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(nameof(IncidentReport.Attachments))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
