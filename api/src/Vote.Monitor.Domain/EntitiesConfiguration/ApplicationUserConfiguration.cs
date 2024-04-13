using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Status).IsRequired();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.RefreshToken).HasMaxLength(256);
        builder.Property(u => u.RefreshTokenExpiryTime);
        builder.Property(u => u.InvitationToken);

        builder.OwnsOne(u => u.Preferences, b =>
        {
            b.ToJson();
        });
    }
}
