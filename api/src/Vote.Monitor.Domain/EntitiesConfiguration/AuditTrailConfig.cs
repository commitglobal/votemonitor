using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class AuditTrailConfig : IEntityTypeConfiguration<Trail>
{
    public void Configure(EntityTypeBuilder<Trail> builder)
    {
        builder.ToTable("AuditTrails");

        builder.HasKey(u => u.Id);
        builder.HasIndex(u => u.UserId);

        builder.UseTptMappingStrategy();

        builder.Property(u => u.Id).IsRequired();
        builder.Property(u => u.UserId).IsRequired();
        builder.Property(u => u.CreatedOn).IsRequired();
        builder.Property(u => u.Type);
        builder.Property(u => u.TableName);
        builder.Property(u => u.Timestamp).IsRequired();
        builder.Property(u => u.OldValues);
        builder.Property(u => u.NewValues);
        builder.Property(u => u.AffectedColumns);
        builder.Property(u => u.PrimaryKey);
    }
}
