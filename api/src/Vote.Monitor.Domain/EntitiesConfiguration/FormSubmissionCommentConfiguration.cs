using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.FormSubmissionCommentAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class FormSubmissionCommentConfiguration : IEntityTypeConfiguration<FormSubmissionComment>
{
    public void Configure(EntityTypeBuilder<FormSubmissionComment> builder)
    {
        builder.ToTable(Tables.FormSubmissionComments);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.HasIndex(x => new { x.ElectionRoundId, x.SubmissionId });

        builder.Property(x => x.QuestionId);
        builder.Property(x => x.Text)
            .HasMaxLength(10000)
            .IsRequired();

        builder.HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);
        
        builder.HasOne(x => x.Submission)
            .WithMany()
            .HasForeignKey(x => x.SubmissionId);
    }
}
