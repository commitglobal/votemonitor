using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.NoteAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable(Tables.Notes);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.MonitoringObserverId);
        builder.HasIndex(x => x.FormId);
        builder.Property(x => x.QuestionId).IsRequired();

        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();
        
        builder.Property(x => x.LastUpdatedAt).IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.MonitoringObserver)
            .WithMany()
            .HasForeignKey(x => x.MonitoringObserverId);

        builder.HasOne(x => x.Form)
            .WithMany()
            .HasForeignKey(x => x.FormId);
    }
}
