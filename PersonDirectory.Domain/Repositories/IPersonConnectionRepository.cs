using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Domain.Repositories;

public interface IPersonConnectionRepository : IBaseRepository<PersonConnection>
{
    Task<ICollection<PersonConnection>> GetPersonConnectionsAsync(int personId, CancellationToken cancellationToken = default);
    Task<ICollection<PersonConnection>> GetConnectionsByTypeAsync(int personId, ConnectionType connectionType, CancellationToken cancellationToken = default);
    Task<PersonConnection?> GetConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default);
    Task<bool> ConnectionExistsAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default);
    Task<int> DeleteConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default);
    Task<int> DeletePersonConnectionsAsync(int personId, CancellationToken cancellationToken = default);

    Task AddBidirectionalConnectionAsync(int personId, int connectedPersonId, ConnectionType connectionType, CancellationToken cancellationToken = default);
    Task DeleteBidirectionalConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default);
}
