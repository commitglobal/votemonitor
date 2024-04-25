using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormSubmissionConfiguration : IEntityTypeConfiguration<FormSubmission>
{
    public void Configure(EntityTypeBuilder<FormSubmission> builder)
    {
        builder.ToTable(Tables.FormSubmissions);

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new
        {
            x.ElectionRoundId,
            x.PollingStationId,
            x.MonitoringObserverId,
            x.FormId
        }).IsUnique();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);
        
        builder.HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);

        builder.HasOne(x => x.Form)
            .WithMany()
            .HasForeignKey(x => x.FormId);

        builder.Property(x => x.Answers)
            .HasConversion<AnswersToJsonConverter, AnswersValueComparer>()
            .HasColumnType("jsonb");
    }
}
