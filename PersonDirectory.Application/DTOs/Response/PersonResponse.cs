namespace PersonDirectory.Application.DTOs.Response;

public class PersonResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string PersonalNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int Age { get; set; }
    public CityResponse City { get; set; } = new();
    public string? ImagePath { get; set; }
    public ICollection<PhoneNumberResponse> PhoneNumbers { get; set; } = new List<PhoneNumberResponse>();
    public ICollection<PersonConnectionResponse> Connections { get; set; } = new List<PersonConnectionResponse>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PersonListResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalNumber { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
}

public class PhoneNumberResponse
{
    public int Id { get; set; }
    public PhoneType Type { get; set; }
    public string Number { get; set; } = string.Empty;
}

public class PersonConnectionResponse
{
    public int Id { get; set; }
    public int ConnectedPersonId { get; set; }
    public string ConnectedPersonFirstName { get; set; } = string.Empty;
    public string ConnectedPersonLastName { get; set; } = string.Empty;
    public ConnectionType ConnectionType { get; set; }
}

public class CityResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}