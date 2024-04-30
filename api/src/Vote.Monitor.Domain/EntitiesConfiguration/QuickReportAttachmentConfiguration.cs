using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class QuickReportAttachmentConfiguration : IEntityTypeConfiguration<QuickReportAttachment>
{
    public void Configure(EntityTypeBuilder<QuickReportAttachment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.FileName).HasMaxLength(256);

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

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);
    }
}
