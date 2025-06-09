using Microsoft.EntityFrameworkCore;
using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Infrastructure.Persistence;

namespace PersonDirectory.Infrastructure.Repositories;

public class PhoneNumberRepository : BaseRepository<PhoneNumber>, IPhoneNumberRepository
{
    public PhoneNumberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<PhoneNumber>> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(pn => pn.PersonId == personId)
            .OrderBy(pn => pn.Type)
            .ToListAsync(cancellationToken);
    }

    public async Task<ICollection<PhoneNumber>> GetByPersonIdAndTypeAsync(int personId, PhoneType phoneType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(pn => pn.PersonId == personId && pn.Type == phoneType)
            .ToListAsync(cancellationToken);
    }

    public async Task<PhoneNumber?> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(pn => pn.Person)
            .FirstOrDefaultAsync(pn => pn.Number == number, cancellationToken);
    }

    public async Task<int> DeleteByPersonIdAsync(int personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(pn => pn.PersonId == personId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}