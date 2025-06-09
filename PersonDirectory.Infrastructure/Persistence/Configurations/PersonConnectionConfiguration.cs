using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonDirectory.Domain.Entities;

namespace PersonDirectory.Infrastructure.Persistence.Configurations;
public class PersonConnectionConfiguration : IEntityTypeConfiguration<PersonConnection>
{
    public void Configure(EntityTypeBuilder<PersonConnection> builder)
    {
        // Primary Key
        builder.HasKey(pc => pc.Id);

        // Properties
        builder.Property(pc => pc.ConnectionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pc => pc.CreatedAt)
            .IsRequired();

        builder.Property(pc => pc.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(pc => new { pc.PersonId, pc.ConnectedPersonId })
            .IsUnique();

        builder.HasIndex(pc => pc.PersonId);
        builder.HasIndex(pc => pc.ConnectedPersonId);
    }
}