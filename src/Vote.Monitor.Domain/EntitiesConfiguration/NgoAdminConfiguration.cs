using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class NgoAdminConfiguration : IEntityTypeConfiguration<NgoAdmin>
{
    public void Configure(EntityTypeBuilder<NgoAdmin> builder)
    {
        builder.ToTable("NgoAdmins");

        builder
            .HasOne(x => x.Ngo)
            .WithMany(x => x.Admins)
            .HasForeignKey(x => x.NgoId);
    }
}
