using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormTemplateConfiguration : IEntityTypeConfiguration<FormTemplate>
{
    public void Configure(EntityTypeBuilder<FormTemplate> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(x => x.Name)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<TranslatedString>(v, (JsonSerializerOptions)null),
                new ValueComparer<TranslatedString>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToDictionary() as TranslatedString))
            .HasColumnType("jsonb");

        builder.Property(x => x.FormType).IsRequired();
        builder.Property(x => x.Code).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder
            .HasMany(x => x.Languages)
            .WithMany();

        builder.Property(x => x.Sections)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<IReadOnlyList<FormSection>>(v, (JsonSerializerOptions)null),
                new ValueComparer<IReadOnlyList<FormSection>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly())
                )
            .HasColumnType("jsonb");
    }
}
