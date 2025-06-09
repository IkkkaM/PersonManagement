using Microsoft.EntityFrameworkCore;
using PersonDirectory.Domain.Commons;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace PersonDirectory.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task AddRangeAsync(ICollection<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public T Update(T entity)
    {
        var entry = _dbSet.Update(entity);
        return entry.Entity;
    }

    public void UpdateRange(ICollection<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(ICollection<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultWithTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TResult?> FirstOrDefaultMappedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).Select(select).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> FirstOrDefaultSortedAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>? sortPredicate, bool isDescending, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = _dbSet.AsNoTracking().Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        if (sortPredicate != null)
        {
            query = isDescending ? query.OrderByDescending(sortPredicate) : query.OrderBy(sortPredicate);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> FirstOrDefaultIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = _dbSet.AsNoTracking().Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> FirstOrDefaultWithTrackingIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = _dbSet.Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = _dbSet.AsNoTracking().Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.SingleOrDefaultAsync();
    }

    public async Task<ICollection<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<ICollection<T>> GetWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<T>> GetWhereIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = _dbSet.AsNoTracking().Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync();
    }

    public async Task<ICollection<T>> GetWhereAsTrackingIncludedAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        var query = _dbSet.Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return await query.ToListAsync();
    }

    public async Task<ICollection<T>> GetWhereAsTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<TResult>> GetMappedAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(predicate).Select(select).ToListAsync(cancellationToken);
    }

    public async Task<ICollection<TResult>> GetAsync<TResult>(Expression<Func<T, TResult>> select)
    {
        return await _dbSet.AsNoTracking().Select(select).ToListAsync();
    }

    public async Task<ICollection<TResult>> GetSortedAsync<TResult>(Expression<Func<T, TResult>> select, Expression<Func<T, object>>? sortPredicate, bool isDescending)
    {
        var query = _dbSet.AsNoTracking();

        if (sortPredicate != null)
        {
            query = isDescending ? query.OrderByDescending(sortPredicate) : query.OrderBy(sortPredicate);
        }

        return await query.Select(select).ToListAsync();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(cancellationToken);
    }

    public async Task<int> CountWithPredicateAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> ExecuteDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<PagedEntities<T>> GetPaginatedAsync(int pageNumber, int itemsPerPage, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);
        var items = await _dbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToListAsync(cancellationToken);

        return new PagedEntities<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = itemsPerPage
        };
    }

    public async Task<PagedEntities<T>> GetPaginatedWhereAsync(Expression<Func<T, bool>> predicate, int pageNumber, int itemsPerPage, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(predicate);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .ToListAsync(cancellationToken);

        return new PagedEntities<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = itemsPerPage
        };
    }

    public async Task<PagedEntities<TResult>> GetPaginatedMappedAsync<TResult>(int pageNumber, int itemsPerPage, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);
        var items = await _dbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(select)
            .ToListAsync(cancellationToken);

        return new PagedEntities<TResult>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = itemsPerPage
        };
    }

    public async Task<PagedEntities<TResult>> GetPaginatedWhereMappedAsync<TResult>(Expression<Func<T, bool>> predicate, int pageNumber, int itemsPerPage, Expression<Func<T, TResult>> select, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking().Where(predicate);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Select(select)
            .ToListAsync(cancellationToken);

        return new PagedEntities<TResult>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = itemsPerPage
        };
    }
}
