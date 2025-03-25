using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;
using Vote.Monitor.Domain.ValueComparers;
using Vote.Monitor.Domain.ValueConverters;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ExportedDataConfiguration : IEntityTypeConfiguration<ExportedData>
{
    public void Configure(EntityTypeBuilder<ExportedData> builder)
    {
        builder.ToTable(Tables.ExportedData);
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.FileName).HasMaxLength(256);
        builder.Property(e => e.StartedAt).IsRequired();
        builder.Property(e => e.ExportStatus).IsRequired();
        builder.Property(e => e.ExportedDataType).IsRequired();
        builder.Property(e => e.Base64EncodedData);
        builder.Property(e => e.CompletedAt);
        
        builder.Property(x => x.FormSubmissionsFilters)
            .HasConversion<ExportFormSubmissionsFiltersToJsonConverter, ExportFormSubmissionsFiltersValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.QuickReportsFilters)
            .HasConversion<ExportQuickReportsFiltersToJsonConverter, ExportQuickReportsFiltersValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.IncidentReportsFilters)
            .HasConversion<ExportIncidentReportsFiltersToJsonConverter, ExportIncidentReportsFiltersValueComparer>()
            .HasColumnType("jsonb");

        builder.Property(x => x.CitizenReportsFilers)
            .HasConversion<ExportCitizenReportsFilersToJsonConverter, ExportCitizenReportsFilersValueComparer>()
            .HasColumnType("jsonb");
        
        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId);
    }
}
