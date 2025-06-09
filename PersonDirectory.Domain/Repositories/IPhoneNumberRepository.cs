using PersonDirectory.Domain.Entities;
using PersonDirectory.Domain.Enums;

namespace PersonDirectory.Domain.Repositories;

public interface IPhoneNumberRepository : IBaseRepository<PhoneNumber>
{
    Task<ICollection<PhoneNumber>> GetByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
    Task<ICollection<PhoneNumber>> GetByPersonIdAndTypeAsync(int personId, PhoneType phoneType, CancellationToken cancellationToken = default);
    Task<PhoneNumber?> GetByNumberAsync(string number, CancellationToken cancellationToken = default);
    Task<int> DeleteByPersonIdAsync(int personId, CancellationToken cancellationToken = default);
}
