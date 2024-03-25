﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.AnswerAttachmentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class AnswerAttachmentConfiguration : IEntityTypeConfiguration<AnswerAttachment>
{
    public void Configure(EntityTypeBuilder<AnswerAttachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.MonitoringObserverId);

        builder.Property(x => x.Filename)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.MimeType)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);

        builder.HasOne(x => x.Form)
            .WithMany()
            .HasForeignKey(x => x.FormId);

        builder.Property(x => x.QuestionId);
    }
}
