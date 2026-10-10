using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.NgoStaffAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class NgoStaffConfiguration : IEntityTypeConfiguration<NgoStaff>
{
    public void Configure(EntityTypeBuilder<NgoStaff> builder)
    {
        builder.ToTable("NgoStaff");
        builder.HasOne(x => x.ApplicationUser);

        builder
            .HasOne(x => x.Ngo)
            .WithMany(x => x.Staff)
            .HasForeignKey(x => x.NgoId);
    }
}
