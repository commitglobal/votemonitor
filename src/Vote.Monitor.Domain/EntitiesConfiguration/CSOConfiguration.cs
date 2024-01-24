using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CSOConfiguration : IEntityTypeConfiguration<CSO>
{
    public void Configure(EntityTypeBuilder<CSO> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(256).IsRequired();
        builder.Property(c => c.Status).IsRequired();
    }
}
