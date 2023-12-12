using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.LanguageAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.NativeName).IsRequired();
        builder.Property(e => e.Iso1).HasMaxLength(2).IsRequired();

        builder.HasData(LanguagesList.GetAll().Select(x => x.ToEntity()));
    }
}
