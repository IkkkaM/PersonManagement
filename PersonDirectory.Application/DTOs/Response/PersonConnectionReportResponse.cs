namespace PersonDirectory.Application.DTOs.Response;

public class PersonConnectionReportResponse
{
    public ICollection<PersonConnectionSummary> PersonConnections { get; set; } = new List<PersonConnectionSummary>();
}

public class PersonConnectionSummary
{
    public int PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Dictionary<ConnectionType, int> ConnectionCounts { get; set; } = new();
    public int TotalConnections { get; set; }
}