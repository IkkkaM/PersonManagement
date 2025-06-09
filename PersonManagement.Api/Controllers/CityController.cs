namespace PersonDirectory.Api.Controllers;

public class CityController : BaseApiController
{
    private readonly ICityService _cityService;

    public CityController(ICityService cityService, IStringLocalizer<CityController> localizer) : base(localizer)
    {
        _cityService = cityService ?? throw new ArgumentNullException(nameof(cityService));
    }

    /// <summary>
    /// Get all cities
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllCities(CancellationToken cancellationToken = default)
    {
        var result = await _cityService.GetAllCitiesAsync(cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get city by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCity(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid city ID" });

        var result = await _cityService.GetCityByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound("City not found");

        return HandleResult(result);
    }
}