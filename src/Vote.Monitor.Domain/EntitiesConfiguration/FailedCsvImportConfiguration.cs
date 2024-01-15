using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ImportValidationErrorsConfiguration : IEntityTypeConfiguration<ImportValidationErrors>
{
    public void Configure(EntityTypeBuilder<ImportValidationErrors> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.ImportType).IsRequired();
        builder.Property(e => e.OriginalFileName).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Data).IsRequired();
    }
}
