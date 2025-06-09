namespace PersonDirectory.Domain.Repositories.UOW;

public interface IUnitOfWork : IDisposable
{
    IPersonRepository PersonRepository { get; }
    ICityRepository CityRepository { get; }
    IPhoneNumberRepository PhoneNumberRepository { get; }
    IPersonConnectionRepository PersonConnectionRepository { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}