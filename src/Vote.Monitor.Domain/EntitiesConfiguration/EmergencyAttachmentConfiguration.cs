using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.EmergencyAttachmentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class EmergencyAttachmentConfiguration : IEntityTypeConfiguration<EmergencyAttachment>
{
    public void Configure(EntityTypeBuilder<EmergencyAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

        builder.Property(x => x.Filename)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasMaxLength(256)
            .IsRequired();
    }
}
