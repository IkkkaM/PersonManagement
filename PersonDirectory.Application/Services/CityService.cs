namespace PersonDirectory.Application.Services;

public class CityService : ICityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<CityService> _localizer;

    public CityService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<CityService> localizer)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Result<List<CityResponse>>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var cities = await _unitOfWork.CityRepository.GetAllAsync(cancellationToken);
            var response = _mapper.Map<List<CityResponse>>(cities);
            return Result<List<CityResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<List<CityResponse>>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<CityResponse>> GetCityByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var city = await _unitOfWork.CityRepository.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
            if (city == null)
                return Result<CityResponse>.Failure(_localizer[ErrorMessages.CityNotFound]);

            var response = _mapper.Map<CityResponse>(city);
            return Result<CityResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CityResponse>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }
}
