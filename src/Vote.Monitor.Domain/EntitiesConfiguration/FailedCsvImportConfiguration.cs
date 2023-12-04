using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;


namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ImportValidationErrorsConfiguration : IEntityTypeConfiguration<ImportValidationErrors>
{
    public void Configure(EntityTypeBuilder<ImportValidationErrors> builder)
    {
        builder
          .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.ImportType).IsRequired();
        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.OriginalFileName).IsRequired();
        builder.Property(e => e.Data).IsRequired();

    }
}
