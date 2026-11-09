using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.AnswerHistoryAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class AnswerChangeConfiguration : IEntityTypeConfiguration<AnswerChange>
{
    public void Configure(EntityTypeBuilder<AnswerChange> builder)
    {
        builder.ToTable(Tables.AnswerChanges);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).IsRequired();
        builder.Property(x => x.RecordType).IsRequired();
        builder.Property(x => x.RecordId).IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.ChangedAt).IsRequired();
        builder.Property(x => x.PreviousAnswerJson).HasColumnName("PreviousAnswer").HasColumnType("jsonb");
        builder.Property(x => x.CurrentAnswerJson).HasColumnName("CurrentAnswer").HasColumnType("jsonb");
        builder.Ignore(x => x.PreviousAnswer);
        builder.Ignore(x => x.CurrentAnswer);

        builder.HasIndex(x => new { x.RecordType, x.RecordId, x.ChangedAt });

        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
