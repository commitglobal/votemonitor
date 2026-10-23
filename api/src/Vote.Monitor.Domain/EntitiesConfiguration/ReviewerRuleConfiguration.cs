using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class ReviewerRuleConfiguration : AutomationRuleConfiguration<ReviewerRule>
{
    public override void Configure(EntityTypeBuilder<ReviewerRule> builder)
    {
        base.Configure(builder);
        builder.ToTable(Tables.ReviewerRules);

        builder.Property(x => x.StageIds).IsRequired();
        builder.Property(x => x.ReviewerIds).IsRequired();
    }
}
