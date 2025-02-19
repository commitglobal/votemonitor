using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ElectionRoundFormTemplateConfiguration : IEntityTypeConfiguration<ElectionRoundFormTemplate>
{
    public void Configure(EntityTypeBuilder<ElectionRoundFormTemplate> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.ElectionRoundId, x.FormTemplateId }).IsUnique();

        builder
            .HasOne(x => x.ElectionRound)
            .WithMany()
            .HasForeignKey(x => x.ElectionRoundId);

        builder.HasOne(x => x.FormTemplate)
            .WithMany()
            .HasForeignKey(x => x.FormTemplateId);
    }
}
