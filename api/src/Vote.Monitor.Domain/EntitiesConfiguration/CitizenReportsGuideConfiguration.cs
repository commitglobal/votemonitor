using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CitizenReportsGuideConfiguration : IEntityTypeConfiguration<CitizenGuide>
{
    public void Configure(EntityTypeBuilder<CitizenGuide> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.FileName)
            .HasMaxLength(256);

        builder.Property(x => x.UploadedFileName)
            .HasMaxLength(256);

        builder.Property(x => x.FilePath)
            .HasMaxLength(256);

        builder.Property(x => x.MimeType)
            .HasMaxLength(256);

        builder.Property(x => x.IsDeleted);

        builder.Property(x => x.Title)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(x => x.WebsiteUrl)
            .HasMaxLength(2048);

        builder
            .Property(x => x.Text);

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);
    }
}
