namespace PersonDirectory.Application.DTOs.Request;

public class PersonConnectionRequest
{
    public int PersonId { get; set; }
    public int ConnectedPersonId { get; set; }
    public ConnectionType ConnectionType { get; set; }
}