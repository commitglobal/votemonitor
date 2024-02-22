using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.Login).IsUnique();
        builder.UseTptMappingStrategy();

        builder.Property(u => u.Id).IsRequired();
        builder.Property(u => u.Name).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Login).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Password).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Role).IsRequired();
        builder.Property(u => u.Status).IsRequired();

        builder.OwnsOne(u => u.Preferences, b =>
        {
            b.ToTable("UserPreferences");
        });
    }
}
