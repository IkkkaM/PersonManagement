using Microsoft.EntityFrameworkCore.Storage;
using PersonDirectory.Domain.Repositories;
using PersonDirectory.Domain.Repositories.UOW;
using PersonDirectory.Infrastructure.Persistence;

namespace PersonDirectory.Infrastructure.Repositories.UOW;


public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IPersonRepository? _personRepository;
    private ICityRepository? _cityRepository;
    private IPhoneNumberRepository? _phoneNumberRepository;
    private IPersonConnectionRepository? _personConnectionRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public IPersonRepository PersonRepository =>
        _personRepository ??= new PersonRepository(_context);

    public ICityRepository CityRepository =>
        _cityRepository ??= new CityRepository(_context);

    public IPhoneNumberRepository PhoneNumberRepository =>
        _phoneNumberRepository ??= new PhoneNumberRepository(_context);

    public IPersonConnectionRepository PersonConnectionRepository =>
        _personConnectionRepository ??= new PersonConnectionRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}