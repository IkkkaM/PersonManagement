using PersonDirectory.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PersonDirectory.Domain.Entities;

public class PhoneNumber : BaseEntity
{
    [Required]
    public PhoneType Type { get; private set; }

    [Required]
    [StringLength(50, MinimumLength = 4)]
    public string Number { get; private set; }

    [Required]
    public int PersonId { get; private set; }

    // Navigation property
    public virtual Person Person { get; private set; }

    protected PhoneNumber()
    {
    }

    public PhoneNumber(PhoneType type, string number, int personId) : this()
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(number));

        if (personId <= 0)
            throw new ArgumentException("PersonId must be greater than 0", nameof(personId));

        Type = type;
        Number = number.Trim();
        PersonId = personId;
    }

    public void UpdatePhoneInfo(PhoneType type, string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Phone number cannot be null or empty", nameof(number));

        Type = type;
        Number = number.Trim();
        SetUpdatedAt();
    }
}
