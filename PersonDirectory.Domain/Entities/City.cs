using System.ComponentModel.DataAnnotations;

namespace PersonDirectory.Domain.Entities;

public class City : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; private set; }

    public virtual ICollection<Person> Persons { get; private set; }

    protected City()
    {
        Persons = new HashSet<Person>();
    }

    public City(string name) : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be null or empty", nameof(name));

        Name = name.Trim();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("City name cannot be null or empty", nameof(name));

        Name = name.Trim();
        SetUpdatedAt();
    }
}