using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class PollingStationInformationFormConfiguration : IEntityTypeConfiguration<PollingStationInformationForm>
{
    public void Configure(EntityTypeBuilder<PollingStationInformationForm> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .HasOne(x => x.ElectionRound)
            .WithOne()
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        var jsonSerializerOptions = (JsonSerializerOptions)null;

        builder.Ignore(x => x.Languages);
        //builder.Property(x => x.Languages).IsRequired();

        builder.Property(x => x.Questions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<BaseQuestion>>(v, jsonSerializerOptions),
                new ValueComparer<IReadOnlyList<BaseQuestion>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly())
            )
            .HasColumnType("jsonb");
    }
}
