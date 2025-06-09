using PersonDirectory.Domain.Commons;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Domain.Repositories;

public interface IPersonRepository : IBaseRepository<Person>
{
    Task<Person?> GetPersonWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<Person?> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default);
    Task<bool> IsPersonalNumberUniqueAsync(string personalNumber, int? excludePersonId = null, CancellationToken cancellationToken = default);

    Task<PagedEntities<Person>> QuickSearchAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedEntities<Person>> DetailedSearchAsync(
        string? firstName = null,
        string? lastName = null,
        string? personalNumber = null,
        Gender? gender = null,
        DateTime? dateOfBirthFrom = null,
        DateTime? dateOfBirthTo = null,
        int? cityId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    Task<ICollection<Person>> GetConnectedPersonsAsync(int personId, ConnectionType? connectionType = null, CancellationToken cancellationToken = default);
    Task<bool> ArePersonsConnectedAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default);

    Task<Dictionary<ConnectionType, int>> GetConnectionCountsByTypeAsync(int personId, CancellationToken cancellationToken = default);
    Task<ICollection<PersonConnectionReportItem>> GetAllPersonsConnectionReportAsync(CancellationToken cancellationToken = default);
}

public class PersonConnectionReportItem
{
    public int PersonId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Dictionary<ConnectionType, int> ConnectionCounts { get; set; } = new();
}
