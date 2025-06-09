namespace PersonDirectory.Application.DTOs.Request;

public class PersonUpdateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public string PersonalNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int CityId { get; set; }
    public ICollection<PhoneNumberRequest> PhoneNumbers { get; set; } = new List<PhoneNumberRequest>();
}