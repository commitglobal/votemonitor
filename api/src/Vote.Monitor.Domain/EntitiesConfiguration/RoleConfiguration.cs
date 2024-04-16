using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.HasData(
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("265e94b0-50fe-4546-b21c-83cb7e94aeff"),
                Name = UserRole.PlatformAdmin.Value,
                NormalizedName = UserRole.PlatformAdmin.Value.ToUpperInvariant()
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("3239f803-dda8-408b-93ad-0ed973a04e45"),
                Name = UserRole.NgoAdmin.Value,
                NormalizedName = UserRole.NgoAdmin.Value.ToUpperInvariant()
            },
            new IdentityRole<Guid>
            {
                Id = Guid.Parse("d1cbef39-62e0-4120-a42b-b01b029dc6ad"),
                Name = UserRole.Observer.Value,
                NormalizedName = UserRole.Observer.Value.ToUpperInvariant()
            }
            );
    }
}
