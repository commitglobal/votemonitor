﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.NotificationAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
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

        builder
            .HasMany(x => x.TargetedObservers)
            .WithMany()
            .UsingEntity<MonitoringObserverNotification>(
                r => r.HasOne(x => x.MonitoringObserver).WithMany().HasForeignKey(e => e.MonitoringObserverId),
                l => l.HasOne(x => x.Notification).WithMany().HasForeignKey(e => e.NotificationId),
                j => j.Property(e => e.IsRead));
    }
}