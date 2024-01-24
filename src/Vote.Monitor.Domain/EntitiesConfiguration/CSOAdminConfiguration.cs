using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
