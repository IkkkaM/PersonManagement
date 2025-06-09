using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonDirectory.Domain.Entities;

namespace PersonDirectory.Infrastructure.Persistence.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Gender)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.PersonalNumber)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(p => p.DateOfBirth)
            .IsRequired();

        builder.Property(p => p.ImagePath)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Indexes
        builder.HasIndex(p => p.PersonalNumber)
            .IsUnique();

        builder.HasIndex(p => new { p.FirstName, p.LastName });

        // Relationships
        builder.HasOne(p => p.City)
            .WithMany(c => c.Persons)
            .HasForeignKey(p => p.CityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.PhoneNumbers)
            .WithOne(pn => pn.Person)
            .HasForeignKey(pn => pn.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Connections)
            .WithOne(pc => pc.Person)
            .HasForeignKey(pc => pc.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.ConnectedBy)
            .WithOne(pc => pc.ConnectedPerson)
            .HasForeignKey(pc => pc.ConnectedPersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}