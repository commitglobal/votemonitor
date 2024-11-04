using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(x => x.Code).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Status).IsRequired();
        builder.Property(x => x.FormType).IsRequired();
        builder.Property(x => x.NumberOfQuestions).IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.MonitoringNgo)
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId);

        builder.Property(x => x.Name)
            .HasConversion<TranslatedStringToJsonConverter, TranslatedStringValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.Description)
            .HasConversion<TranslatedStringToJsonConverter, TranslatedStringValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.DefaultLanguage).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Languages).IsRequired();
        builder.Property(x => x.Icon).IsRequired(false);

        builder.Property(x => x.Questions)
            .HasConversion<QuestionsToJsonConverter, QuestionsValueComparer>()
            .HasColumnType("jsonb");
        
        builder.Property(x => x.LanguagesTranslationStatus)
            .HasConversion<LanguagesTranslationStatusToJsonConverter, LanguagesTranslationStatusValueComparer>()
            .HasColumnType("jsonb");
    }
}
