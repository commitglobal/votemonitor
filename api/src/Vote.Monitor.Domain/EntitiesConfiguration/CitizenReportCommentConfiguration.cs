using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class CitizenReportCommentConfiguration : IEntityTypeConfiguration<CitizenReportComment>
{
    public void Configure(EntityTypeBuilder<CitizenReportComment> builder)
    {
        builder.ToTable(Tables.CitizenReportComments);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => new { x.ElectionRoundId, x.CitizenReportId });

        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.CitizenReport)
            .WithMany()
            .HasForeignKey(x => x.CitizenReportId);
    }
}
