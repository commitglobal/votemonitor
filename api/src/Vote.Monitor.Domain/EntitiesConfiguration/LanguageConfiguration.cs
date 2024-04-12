using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasIndex(l => l.Id);
        builder.HasIndex(l => l.Iso1).IsUnique();

        builder.Property(l => l.Name).HasMaxLength(256).IsRequired();
        builder.Property(l => l.NativeName).HasMaxLength(256).IsRequired();
        builder.Property(l => l.Iso1).HasMaxLength(2).IsRequired();

        builder.HasData(LanguagesList.GetAll().Select(x => x.ToEntity()));
    }
}
