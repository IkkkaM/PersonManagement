using Microsoft.EntityFrameworkCore;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Infrastructure.Persistence;

namespace PersonDirectory.Infrastructure.Repositories;

public class CityRepository : BaseRepository<City>, ICityRepository
{
    public CityRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<bool> IsCityNameUniqueAsync(string name, int? excludeCityId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(c => c.Name == name);

        if (excludeCityId.HasValue)
        {
            query = query.Where(c => c.Id != excludeCityId.Value);
        }

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<ICollection<City>> GetCitiesWithPersonCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(c => c.Persons)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
}