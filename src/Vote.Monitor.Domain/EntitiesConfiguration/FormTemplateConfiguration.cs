using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormTemplateConfiguration : IEntityTypeConfiguration<FormTemplate>
{
    public void Configure(EntityTypeBuilder<FormTemplate> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(x => x.FormTemplateType).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(256).IsRequired();
        builder.Property(x => x.DefaultLanguage).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        var jsonSerializerOptions = (JsonSerializerOptions)null;

        builder.Property(x => x.Name)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<TranslatedString>(v, jsonSerializerOptions),
                new ValueComparer<TranslatedString>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary() as TranslatedString))
            .HasColumnType("jsonb");

        builder.Property(x => x.Languages).IsRequired();

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
