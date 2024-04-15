using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasIndex(c => c.Iso2).IsUnique();
        builder.HasIndex(c => c.Iso3).IsUnique();
        builder.HasIndex(c => c.NumericCode).IsUnique();

        builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
        builder.Property(c => c.FullName).HasMaxLength(256).IsRequired();
        builder.Property(c => c.NumericCode).HasMaxLength(3).HasMaxLength(3).IsRequired();
        builder.Property(c => c.Iso2).HasMaxLength(2).HasMaxLength(2).IsRequired();
        builder.Property(c => c.Iso3).HasMaxLength(3).IsRequired();

        builder.HasData(CountriesList.GetAll().Select(x => x.ToEntity()));
    }
}
