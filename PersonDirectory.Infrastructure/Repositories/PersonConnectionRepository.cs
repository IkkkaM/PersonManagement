using Microsoft.EntityFrameworkCore;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Infrastructure.Persistence;

namespace PersonDirectory.Infrastructure.Repositories;

public class PersonConnectionRepository : BaseRepository<PersonConnection>, IPersonConnectionRepository
{
    public PersonConnectionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<PersonConnection>> GetPersonConnectionsAsync(int personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(pc => pc.ConnectedPerson)
            .Where(pc => pc.PersonId == personId)
            .OrderBy(pc => pc.ConnectedPerson.FirstName)
            .ThenBy(pc => pc.ConnectedPerson.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<PersonConnection>> GetConnectionsByTypeAsync(int personId, ConnectionType connectionType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(pc => pc.ConnectedPerson)
            .Where(pc => pc.PersonId == personId && pc.ConnectionType == connectionType)
            .OrderBy(pc => pc.ConnectedPerson.FirstName)
            .ThenBy(pc => pc.ConnectedPerson.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<PersonConnection?> GetConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(pc => pc.PersonId == personId && pc.ConnectedPersonId == connectedPersonId, cancellationToken);
    }

    public async Task<bool> ConnectionExistsAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(pc => pc.PersonId == personId && pc.ConnectedPersonId == connectedPersonId, cancellationToken);
    }

    public async Task<int> DeleteConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pc => pc.PersonId == personId && pc.ConnectedPersonId == connectedPersonId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<int> DeletePersonConnectionsAsync(int personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pc => pc.PersonId == personId || pc.ConnectedPersonId == personId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task AddBidirectionalConnectionAsync(int personId, int connectedPersonId, ConnectionType connectionType, CancellationToken cancellationToken = default)
    {
        var connections = new[]
        {
                new PersonConnection(personId, connectedPersonId, connectionType),
                new PersonConnection(connectedPersonId, personId, connectionType)
            };

        await _dbSet.AddRangeAsync(connections, cancellationToken);
    }

    public async Task DeleteBidirectionalConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default)
    {
        await _dbSet
            .Where(pc =>
                (pc.PersonId == personId && pc.ConnectedPersonId == connectedPersonId) ||
                (pc.PersonId == connectedPersonId && pc.ConnectedPersonId == personId))
            .ExecuteDeleteAsync(cancellationToken);
    }
}