using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Domain.EntitiesConfiguration;

public class FormConfiguration : IEntityTypeConfiguration<Form>
{
    public void Configure(EntityTypeBuilder<Form> builder)
    {
        builder
            .Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v4()");

        builder.Property(e => e.Code).IsRequired();
        builder.Property(e => e.LanguageId).IsRequired();
        builder.Property(e => e.Description).IsRequired();
        builder.Property(e => e.Status).IsRequired();

        builder.OwnsMany(f => f.Questions, q =>
        {
            q.ToJson();
        });
    }
}
