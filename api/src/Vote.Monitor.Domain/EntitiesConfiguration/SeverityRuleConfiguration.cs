using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

internal class SeverityRuleConfiguration : AutomationRuleConfiguration<SeverityRule>
{
    public override void Configure(EntityTypeBuilder<SeverityRule> builder)
    {
        base.Configure(builder);
        builder.ToTable(Tables.SeverityRules);

        builder.Property(x => x.Severity).IsRequired();
    }
}
