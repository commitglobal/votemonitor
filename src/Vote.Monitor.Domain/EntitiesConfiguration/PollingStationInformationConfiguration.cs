using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class PollingStationInformationConfiguration : IEntityTypeConfiguration<PollingStationInformation>
{
    public void Configure(EntityTypeBuilder<PollingStationInformation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);
        
        builder.HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);

        var jsonSerializerOptions = (JsonSerializerOptions)null;

        builder.Property(x => x.Answers)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<BaseAnswer>>(v, jsonSerializerOptions),
                new ValueComparer<IReadOnlyList<BaseAnswer>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly())
            )
            .HasColumnType("jsonb");
    }
}
