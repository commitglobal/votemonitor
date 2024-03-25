using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.PollingStationAttachmentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class PollingStationAttachmentConfiguration : IEntityTypeConfiguration<PollingStationAttachment>
{
    public void Configure(EntityTypeBuilder<PollingStationAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.MonitoringObserverId);
        builder.HasIndex(x => x.PollingStationId);

        builder.Property(x => x.FileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.UploadedFileName)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.FilePath)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.IsDeleted);
        
        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);
    }
}
