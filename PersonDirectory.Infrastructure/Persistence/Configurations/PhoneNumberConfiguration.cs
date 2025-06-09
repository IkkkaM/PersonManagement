using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonDirectory.Domain.Entities;

namespace PersonDirectory.Infrastructure.Persistence.Configurations;

public class PhoneNumberConfiguration : IEntityTypeConfiguration<PhoneNumber>
{
    public void Configure(EntityTypeBuilder<PhoneNumber> builder)
    {
        // Primary Key
        builder.HasKey(pn => pn.Id);

        // Properties
        builder.Property(pn => pn.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(pn => pn.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(pn => pn.CreatedAt)
            .IsRequired();

        builder.Property(pn => pn.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(pn => pn.PersonId);
        builder.HasIndex(pn => pn.Number);
    }
}