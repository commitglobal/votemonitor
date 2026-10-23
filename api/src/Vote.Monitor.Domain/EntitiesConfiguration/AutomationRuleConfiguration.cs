using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

/// <summary>
/// The columns every kind of automation rule has. Names are unique per monitoring NGO and kind, ignoring case: a unique
/// index on <c>("MonitoringNgoId", lower("Name"))</c>, created by the migration since EF can't express it.
/// </summary>
internal abstract class AutomationRuleConfiguration<TRule> : IEntityTypeConfiguration<TRule> where TRule : AutomationRule
{
    public virtual void Configure(EntityTypeBuilder<TRule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
        builder.Property(x => x.RecordTypes)
            .IsRequired()
            .HasConversion(
                x => x.Select(recordType => recordType.Value).ToArray(),
                x => x.Select(ReportRecordType.FromValue).ToArray(),
                new ValueComparer<ReportRecordType[]>(
                    (a, b) => a!.SequenceEqual(b!),
                    x => x.Aggregate(0, (hash, recordType) => HashCode.Combine(hash, recordType.GetHashCode())),
                    x => x.ToArray()))
            .HasColumnType("text[]");
        builder.Property(x => x.Conditions).HasColumnType("jsonb");
        builder.Ignore(x => x.Kind);

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
