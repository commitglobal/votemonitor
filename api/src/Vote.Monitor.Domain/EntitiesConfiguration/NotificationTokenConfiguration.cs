using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.NotificationTokenAggregate;
using Vote.Monitor.Domain.Entities.ObserverAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class NotificationTokenConfiguration : IEntityTypeConfiguration<NotificationToken>
{
    public void Configure(EntityTypeBuilder<NotificationToken> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).IsRequired();
        builder.Property(x => x.Token).IsRequired().HasMaxLength(1024);

        builder.HasOne<Observer>().WithMany().HasForeignKey(x => x.ObserverId);
    }
}
