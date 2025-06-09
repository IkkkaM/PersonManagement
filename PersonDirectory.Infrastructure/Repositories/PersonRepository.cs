using Microsoft.EntityFrameworkCore;
using PersonDirectory.Domain.Commons;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Infrastructure.Persistence;

namespace PersonDirectory.Infrastructure.Repositories;

public class PersonRepository : BaseRepository<Person>, IPersonRepository
{
    public PersonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Person?> GetPersonWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.City)
            .Include(p => p.PhoneNumbers)
            .Include(p => p.Connections)
                .ThenInclude(pc => pc.ConnectedPerson)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Person?> GetByPersonalNumberAsync(string personalNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PersonalNumber == personalNumber, cancellationToken);
    }

    public async Task<bool> IsPersonalNumberUniqueAsync(string personalNumber, int? excludePersonId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.PersonalNumber == personalNumber);

        if (excludePersonId.HasValue)
        {
            query = query.Where(p => p.Id != excludePersonId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<PagedEntities<Person>> QuickSearchAsync(string searchTerm, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(p => p.City)
            .Where(p =>
                EF.Functions.Like(p.FirstName, $"%{searchTerm}%") ||
                EF.Functions.Like(p.LastName, $"%{searchTerm}%") ||
                EF.Functions.Like(p.PersonalNumber, $"%{searchTerm}%"))
            .OrderBy(p => p.FirstName)
            .ThenBy(p => p.LastName);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedEntities<Person>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<PagedEntities<Person>> DetailedSearchAsync(
        string? firstName = null,
        string? lastName = null,
        string? personalNumber = null,
        Gender? gender = null,
        DateTime? dateOfBirthFrom = null,
        DateTime? dateOfBirthTo = null,
        int? cityId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Include(p => p.City)
            .AsQueryable();

        if (!string.IsNullOrEmpty(firstName))
            query = query.Where(p => EF.Functions.Like(p.FirstName, $"%{firstName}%"));

        if (!string.IsNullOrEmpty(lastName))
            query = query.Where(p => EF.Functions.Like(p.LastName, $"%{lastName}%"));

        if (!string.IsNullOrEmpty(personalNumber))
            query = query.Where(p => EF.Functions.Like(p.PersonalNumber, $"%{personalNumber}%"));

        if (gender.HasValue)
            query = query.Where(p => p.Gender == gender.Value);

        if (dateOfBirthFrom.HasValue)
            query = query.Where(p => p.DateOfBirth >= dateOfBirthFrom.Value);

        if (dateOfBirthTo.HasValue)
            query = query.Where(p => p.DateOfBirth <= dateOfBirthTo.Value);

        if (cityId.HasValue)
            query = query.Where(p => p.CityId == cityId.Value);

        query = query.OrderBy(p => p.FirstName).ThenBy(p => p.LastName);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedEntities<Person>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<ICollection<Person>> GetConnectedPersonsAsync(int personId, ConnectionType? connectionType = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PersonConnections
            .AsNoTracking()
            .Where(pc => pc.PersonId == personId);

        if (connectionType.HasValue)
        {
            query = query.Where(pc => pc.ConnectionType == connectionType.Value);
        }

        return await query
            .Include(pc => pc.ConnectedPerson)
                .ThenInclude(p => p.City)
            .Select(pc => pc.ConnectedPerson)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ArePersonsConnectedAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default)
    {
        return await _context.PersonConnections
            .AnyAsync(pc => pc.PersonId == personId && pc.ConnectedPersonId == connectedPersonId, cancellationToken);
    }

    public async Task<Dictionary<ConnectionType, int>> GetConnectionCountsByTypeAsync(int personId, CancellationToken cancellationToken = default)
    {
        var connectionCounts = await _context.PersonConnections
            .Where(pc => pc.PersonId == personId)
            .GroupBy(pc => pc.ConnectionType)
            .Select(g => new { ConnectionType = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return connectionCounts.ToDictionary(x => x.ConnectionType, x => x.Count);
    }

    public async Task<ICollection<PersonConnectionReportItem>> GetAllPersonsConnectionReportAsync(CancellationToken cancellationToken = default)
    {
        var persons = await _dbSet
            .AsNoTracking()
            .Select(p => new { p.Id, p.FirstName, p.LastName })
            .ToListAsync(cancellationToken);

        var connections = await _context.PersonConnections
            .AsNoTracking()
            .GroupBy(pc => new { pc.PersonId, pc.ConnectionType })
            .Select(g => new { g.Key.PersonId, g.Key.ConnectionType, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var connectionLookup = connections.ToLookup(c => c.PersonId);

        return persons.Select(p => new PersonConnectionReportItem
        {
            PersonId = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            ConnectionCounts = connectionLookup[p.Id].ToDictionary(c => c.ConnectionType, c => c.Count)
        }).ToList();
    }
}