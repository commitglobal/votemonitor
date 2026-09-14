using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.IncidentReportCommentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class IncidentReportCommentConfiguration : IEntityTypeConfiguration<IncidentReportComment>
{
    public void Configure(EntityTypeBuilder<IncidentReportComment> builder)
    {
        builder.ToTable(Tables.IncidentReportComments);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => new { x.ElectionRoundId, x.IncidentReportId });

        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.IncidentReport)
            .WithMany()
            .HasForeignKey(x => x.IncidentReportId);
    }
}
