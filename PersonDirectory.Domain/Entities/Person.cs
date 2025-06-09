using PersonDirectory.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace PersonDirectory.Domain.Entities;

public class Person : BaseEntity
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string FirstName { get; private set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string LastName { get; private set; }

    [Required]
    public Gender Gender { get; private set; }

    [Required]
    [StringLength(11, MinimumLength = 11)]
    public string PersonalNumber { get; private set; }

    [Required]
    public DateTime DateOfBirth { get; private set; }

    [Required]
    public int CityId { get; private set; }

    [StringLength(500)]
    public string? ImagePath { get; private set; }

    public virtual City City { get; private set; }
    public virtual ICollection<PhoneNumber> PhoneNumbers { get; private set; }
    public virtual ICollection<PersonConnection> Connections { get; private set; }
    public virtual ICollection<PersonConnection> ConnectedBy { get; private set; }

    protected Person()
    {
        // EF Core constructor
        PhoneNumbers = new HashSet<PhoneNumber>();
        Connections = new HashSet<PersonConnection>();
        ConnectedBy = new HashSet<PersonConnection>();
    }

    public Person(string firstName, string lastName, Gender gender,
                 string personalNumber, DateTime dateOfBirth, int cityId) : this()
    {
        ValidateAndSetBasicInfo(firstName, lastName, gender, personalNumber, dateOfBirth, cityId);
    }

    public void UpdateBasicInfo(string firstName, string lastName, Gender gender,
                              string personalNumber, DateTime dateOfBirth, int cityId)
    {
        ValidateAndSetBasicInfo(firstName, lastName, gender, personalNumber, dateOfBirth, cityId);
        SetUpdatedAt();
    }

    public void UpdateImagePath(string? imagePath)
    {
        ImagePath = imagePath?.Trim();
        SetUpdatedAt();
    }

    public void AddPhoneNumber(PhoneType type, string number)
    {
        var phoneNumber = new PhoneNumber(type, number, Id);
        PhoneNumbers.Add(phoneNumber);
        SetUpdatedAt();
    }

    public void RemovePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumbers.Remove(phoneNumber);
        SetUpdatedAt();
    }

    public void ClearPhoneNumbers()
    {
        PhoneNumbers.Clear();
        SetUpdatedAt();
    }

    private void ValidateAndSetBasicInfo(string firstName, string lastName, Gender gender,
                                       string personalNumber, DateTime dateOfBirth, int cityId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName cannot be null or empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName cannot be null or empty", nameof(lastName));

        if (string.IsNullOrWhiteSpace(personalNumber) || personalNumber.Length != 11)
            throw new ArgumentException("PersonalNumber must be exactly 11 characters", nameof(personalNumber));

        if (DateTime.UtcNow.Year - dateOfBirth.Year < 18)
            throw new ArgumentException("Person must be at least 18 years old", nameof(dateOfBirth));

        if (cityId <= 0)
            throw new ArgumentException("CityId must be greater than 0", nameof(cityId));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Gender = gender;
        PersonalNumber = personalNumber.Trim();
        DateOfBirth = dateOfBirth.Date;
        CityId = cityId;
    }

    public int GetAge()
    {
        var today = DateTime.Today;
        var age = today.Year - DateOfBirth.Year;
        if (DateOfBirth.Date > today.AddYears(-age))
            age--;
        return age;
    }
}
