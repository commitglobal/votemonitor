using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    private readonly Guid _electionRoundId;

    public FormConfiguration(Guid electionRoundId)
    {
        _electionRoundId = electionRoundId;
    }

    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder
            .Property(f => f.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(f => f.Code).IsRequired();
        builder.Property(f => f.LanguageId).IsRequired();
        builder.Property(f => f.Description).IsRequired();
        builder.Property(f => f.Status).IsRequired();

        builder.HasIndex(f => f.ElectionRoundId);
        builder.HasOne<ElectionRound>()
            .WithMany()
            .HasForeignKey(f => f.ElectionRoundId)
            .IsRequired();

        builder.OwnsMany(f => f.Questions, q =>
        {
            q.ToJson();
        });

        builder.HasQueryFilter(f => f.ElectionRoundId == _electionRoundId);
    }
}
