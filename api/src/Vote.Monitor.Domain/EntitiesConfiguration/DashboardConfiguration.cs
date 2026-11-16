using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.DashboardAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class DashboardConfiguration : IEntityTypeConfiguration<Dashboard>
{
    public void Configure(EntityTypeBuilder<Dashboard> builder)
    {
        builder.ToTable(Tables.Dashboards);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(Dashboard.MaxNameLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(Dashboard.MaxDescriptionLength);
        builder.Property(x => x.Charts).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByName).HasMaxLength(Dashboard.MaxCreatorNameLength).IsRequired();

        builder.HasIndex(x => new { x.ElectionRoundId, x.MonitoringNgoId });

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<MonitoringNgo>()
            .WithMany()
            .HasForeignKey(x => x.MonitoringNgoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
