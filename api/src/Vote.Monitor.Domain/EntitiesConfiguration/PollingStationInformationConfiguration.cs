using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class PollingStationInformationConfiguration : IEntityTypeConfiguration<PollingStationInformation>
{
    public void Configure(EntityTypeBuilder<PollingStationInformation> builder)
    {
        builder.ToTable(Tables.PollingStationInformation);

        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new
        {
            x.ElectionRoundId,
            x.PollingStationId,
            x.MonitoringObserverId,
            x.PollingStationInformationFormId
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

        builder.Property(x => x.ArrivalTime);
        builder.Property(x => x.DepartureTime);
        builder.Property(x => x.MinutesMonitoring);
        builder.Property(x => x.NumberOfQuestionsAnswered);

        builder.Property(x => x.Answers)
            .HasConversion<AnswersToJsonConverter, AnswersValueComparer>()
            .HasColumnType("jsonb");
    }
}
