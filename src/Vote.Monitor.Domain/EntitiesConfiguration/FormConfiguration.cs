using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(x => x.FormType).IsRequired();
        builder.Property(x => x.Code).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.MonitoringNgo)
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId);

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

        builder
            .Property(x => x.Languages)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<string>>(v, jsonSerializerOptions),
                new ValueComparer<IReadOnlyList<string>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly()))
            .HasColumnType("jsonb");

        builder.Property(x => x.Sections)
            .HasConversion(
                v => JsonSerializer.Serialize(v, jsonSerializerOptions),
                v => JsonSerializer.Deserialize<IReadOnlyList<FormSection>>(v, jsonSerializerOptions),
                new ValueComparer<IReadOnlyList<FormSection>>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly())
                )
            .HasColumnType("jsonb");
    }
}
