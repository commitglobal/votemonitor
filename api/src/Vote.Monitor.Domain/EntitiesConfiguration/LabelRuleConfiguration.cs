using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.RecordLabelAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class LabelRuleConfiguration : AutomationRuleConfiguration<LabelRule>
{
    public override void Configure(EntityTypeBuilder<LabelRule> builder)
    {
        base.Configure(builder);
        builder.ToTable(Tables.LabelRules);

        builder.Property(x => x.Labels).IsRequired();
    }
}
