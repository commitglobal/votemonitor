using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.NotificationStubAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class NotificationStubConfiguration : IEntityTypeConfiguration<NotificationStub>
{
    public void Configure(EntityTypeBuilder<NotificationStub> builder)
    {
        builder.Property(x => x.StubType).IsRequired();
        builder.Property(x => x.SerializedData).IsRequired();
    }
}
