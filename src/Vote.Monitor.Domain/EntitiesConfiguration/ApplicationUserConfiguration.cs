using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.UseTptMappingStrategy();

        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Login).IsRequired();
        builder.Property(e => e.Password).IsRequired();
        builder.Property(e => e.Role).IsRequired();
        builder.Property(e => e.Status).IsRequired();
    }
}
