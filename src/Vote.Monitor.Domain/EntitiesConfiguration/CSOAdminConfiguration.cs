using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class CSOAdminConfiguration : IEntityTypeConfiguration<CSOAdmin>
{
    public void Configure(EntityTypeBuilder<CSOAdmin> builder)
    {
        builder.ToTable("CSOAdmins");

        builder
            .HasOne(x => x.CSO)
            .WithMany(x => x.Admins)
            .HasForeignKey(x => x.CSOId);
    }
}
