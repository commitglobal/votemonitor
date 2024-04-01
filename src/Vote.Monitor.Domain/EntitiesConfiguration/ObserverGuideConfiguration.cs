﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ObserverGuideConfiguration : IEntityTypeConfiguration<ObserverGuide>
{
    public void Configure(EntityTypeBuilder<ObserverGuide> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();

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

        builder.HasOne(x => x.MonitoringNgo)
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId);
    }
}
