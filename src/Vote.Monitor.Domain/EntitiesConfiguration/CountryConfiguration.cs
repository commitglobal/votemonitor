using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.CountryAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.FullName).IsRequired();
        builder.Property(e => e.NumericCode).HasMaxLength(3).IsRequired();
        builder.Property(e => e.Iso2).HasMaxLength(2).IsRequired();
        builder.Property(e => e.Iso3).HasMaxLength(3).IsRequired();

        builder.HasData(CountriesList.GetAll().Select(x => x.ToEntity()));
    }
}
