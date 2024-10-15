using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class QuickReportConfiguration : IEntityTypeConfiguration<QuickReport>
{
    public void Configure(EntityTypeBuilder<QuickReport> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Id);

        builder.Property(e => e.Id).IsRequired();
        builder.Property(e => e.QuickReportLocationType).IsRequired();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(1024);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(10000);

        builder.Property(x => x.FollowUpStatus)
            .IsRequired()
            .HasDefaultValue(QuickReportFollowUpStatus.NotApplicable);
        
        builder.Property(x => x.IncidentCategory)
            .IsRequired()
            .HasDefaultValue(IncidentCategory.Other);

        builder
            .Property(e => e.PollingStationDetails)
            .IsRequired(false)
            .HasMaxLength(1024);

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);

        builder
            .HasOne(x => x.PollingStation)
            .WithMany()
            .HasForeignKey(x => x.PollingStationId)
            .IsRequired(false);
    }
}
