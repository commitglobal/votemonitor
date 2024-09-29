using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.IssueReportAttachmentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class    IssueReportAttachmentConfiguration : IEntityTypeConfiguration<IssueReportAttachment>
{
    public void Configure(EntityTypeBuilder<IssueReportAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.IssueReportId);
        builder.HasIndex(x => x.FormId);
        builder.Property(x => x.QuestionId).IsRequired();

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
    }
}