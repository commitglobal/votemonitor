using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.CitizenNotificationAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CitizenNotificationConfiguration : IEntityTypeConfiguration<CitizenNotification>
{
    public void Configure(EntityTypeBuilder<CitizenNotification> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.HasIndex(e => e.ElectionRoundId);
        builder.HasIndex(e => e.SenderId);
        builder.Property(x => x.Title).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Body).IsRequired();

        builder
            .HasOne(x => x.Sender)
            .WithMany()
            .HasForeignKey(x => x.SenderId);
    }
}