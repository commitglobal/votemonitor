using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.SavedViewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class SavedViewConfiguration : IEntityTypeConfiguration<SavedView>
{
    public void Configure(EntityTypeBuilder<SavedView> builder)
    {
        builder.ToTable(Tables.SavedViews);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(SavedView.MaxNameLength).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(SavedView.MaxDescriptionLength);
        builder.Property(x => x.Access).IsRequired();
        builder.Property(x => x.State).HasColumnType("jsonb").IsRequired();
        builder.Property(x => x.CreatedByName).HasMaxLength(SavedView.MaxCreatorNameLength).IsRequired();

        builder.HasIndex(x => new { x.ElectionRoundId, x.MonitoringNgoId, x.RecordType });

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
