using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.FeedbackAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("UserFeedback");

        builder.HasKey(e => e.Id);
        builder.Property(x => x.TimeSubmitted).IsRequired();
        builder.Property(x => x.UserFeedback).HasMaxLength(10_000).IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.Observer)
            .WithMany()
            .HasForeignKey(x => x.ObserverId);

        builder.Property(x => x.Metadata)
            .HasConversion<DictionaryToJsonConverter, DictionaryValueComparer>()
            .HasColumnType("jsonb");
    }
}
