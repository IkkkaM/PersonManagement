using PersonDirectory.Domain.Entities;

namespace PersonDirectory.Domain.Repositories
{
    public interface ICityRepository : IBaseRepository<City>
    {
        Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> IsCityNameUniqueAsync(string name, int? excludeCityId = null, CancellationToken cancellationToken = default);
        Task<ICollection<City>> GetCitiesWithPersonCountAsync(CancellationToken cancellationToken = default);
    }
}
