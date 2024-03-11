using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class NotificationTokenConfiguration : IEntityTypeConfiguration<NotificationToken>
{
    public void Configure(EntityTypeBuilder<NotificationToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(x => x.Timestamp).IsRequired();

        builder.HasOne<Observer>().WithMany().HasForeignKey(x => x.ObserverId);
    }
}
