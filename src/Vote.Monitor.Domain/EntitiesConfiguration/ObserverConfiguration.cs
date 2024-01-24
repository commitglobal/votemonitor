using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class ObserverConfiguration : IEntityTypeConfiguration<Observer>
{
    public void Configure(EntityTypeBuilder<Observer> builder)
    {
        builder.ToTable("Observers");
    }
}
