using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.QuickReportCommentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class QuickReportCommentConfiguration : IEntityTypeConfiguration<QuickReportComment>
{
    public void Configure(EntityTypeBuilder<QuickReportComment> builder)
    {
        builder.ToTable(Tables.QuickReportComments);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => new { x.ElectionRoundId, x.QuickReportId });

        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.QuickReport)
            .WithMany()
            .HasForeignKey(x => x.QuickReportId);
    }
}
