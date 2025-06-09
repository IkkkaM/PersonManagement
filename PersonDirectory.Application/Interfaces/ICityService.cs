namespace PersonDirectory.Application.Interfaces;

public interface ICityService
{
    Task<Result<List<CityResponse>>> GetAllCitiesAsync(CancellationToken cancellationToken = default);
    Task<Result<CityResponse>> GetCityByIdAsync(int id, CancellationToken cancellationToken = default);
}
