using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.IssueReportNoteAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class IssueReportNoteConfiguration : IEntityTypeConfiguration<IssueReportNote>
{
    public void Configure(EntityTypeBuilder<IssueReportNote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => x.ElectionRoundId);
        builder.HasIndex(x => x.IssueReportId);
        builder.HasIndex(x => x.FormId);
        
        builder.Property(x => x.QuestionId).IsRequired();

        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.Form)
            .WithMany()
            .HasForeignKey(x => x.FormId);
    }
}