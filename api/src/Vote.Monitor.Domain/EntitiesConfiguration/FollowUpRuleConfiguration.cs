using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.FollowUpAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class FollowUpRuleConfiguration : AutomationRuleConfiguration<FollowUpRule>
{
    public override void Configure(EntityTypeBuilder<FollowUpRule> builder)
    {
        base.Configure(builder);
        builder.ToTable(Tables.FollowUpRules);

        builder.Property(x => x.FollowUpStatusId).IsRequired();

        // A status a rule uses can't be deleted; its rules are moved first.
        builder.HasOne<FollowUpStatusDefinition>()
            .WithMany()
            .HasForeignKey(x => x.FollowUpStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
