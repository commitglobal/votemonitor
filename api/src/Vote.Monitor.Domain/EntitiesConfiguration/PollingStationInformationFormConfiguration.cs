using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

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

        builder.Property(x => x.DefaultLanguage).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Languages).IsRequired();
        builder.Property(x => x.NumberOfQuestions).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(256).IsRequired();
        builder.Property(x => x.FormType).IsRequired();

        builder.Property(x => x.Name)
            .HasConversion<TranslatedStringToJsonConverter, TranslatedStringValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.Description)
            .HasConversion<TranslatedStringToJsonConverter, TranslatedStringValueComparer>()
            .HasColumnType("jsonb");


        builder.Property(x => x.Questions)
            .HasConversion<QuestionsToJsonConverter, QuestionsValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.LanguagesTranslationStatus)
            .HasConversion<LanguagesTranslationStatusToJsonConverter, LanguagesTranslationStatusValueComparer>()
            .HasColumnType("jsonb");
    }
}