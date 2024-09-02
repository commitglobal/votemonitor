using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class CitizenReportConfiguration : IEntityTypeConfiguration<CitizenReport>
{
    public void Configure(EntityTypeBuilder<CitizenReport> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new
        {
            x.ElectionRoundId,
            x.FormId
        }).IsUnique();

        builder.Property(x => x.Email).IsRequired(false).HasMaxLength(256);
        builder.Property(x => x.ContactInformation).IsRequired(false).HasMaxLength(2048);
        builder.Property(x => x.NumberOfFlaggedAnswers).IsRequired();
        builder.Property(x => x.NumberOfQuestionsAnswered).IsRequired();
        builder.Property(x => x.FollowUpStatus)
            .IsRequired()
            .HasDefaultValue(CitizenReportFollowUpStatus.NotApplicable);

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.Form)
            .WithMany()
            .HasForeignKey(x => x.FormId);

        builder.Property(x => x.Answers)
            .HasConversion<AnswersToJsonConverter, AnswersValueComparer>()
            .HasColumnType("jsonb");

        builder
            .HasMany(x => x.Notes)
            .WithOne(x => x.CitizenReport)
            .HasForeignKey(x => x.CitizenReportId);

        builder
            .HasMany(x => x.Attachments)
            .WithOne(x => x.CitizenReport)
            .HasForeignKey(x => x.CitizenReportId);

        builder.Navigation(nameof(CitizenReport.Notes))
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(nameof(CitizenReport.Attachments))
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}