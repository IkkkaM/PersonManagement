namespace PersonDirectory.Api.Tests.Controllers;

public class CityControllerTests
{
    private readonly ICityService _cityService;
    private readonly IStringLocalizer<CityController> _localizer;
    private readonly CityController _controller;

    public CityControllerTests()
    {
        _cityService = Substitute.For<ICityService>();
        _localizer = Substitute.For<IStringLocalizer<CityController>>();
        _controller = new CityController(_cityService, _localizer);
    }

    [Fact]
    public async Task GetAllCities_WhenCitiesExist_ReturnsListOfCities()
    {
        // Arrange
        var expectedCities = new List<CityResponse>
            {
                new() { Id = 1, Name = "თბილისი" },
                new() { Id = 2, Name = "ბათუმი" },
                new() { Id = 3, Name = "ქუთაისი" }
            };
        _cityService.GetAllCitiesAsync(Arg.Any<CancellationToken>())
            .Returns(Result<List<CityResponse>>.Success(expectedCities));

        // Act
        var result = await _controller.GetAllCities();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllCities_WhenNoCitiesExist_ReturnsEmptyList()
    {
        // Arrange
        var emptyCities = new List<CityResponse>();
        _cityService.GetAllCitiesAsync(Arg.Any<CancellationToken>())
            .Returns(Result<List<CityResponse>>.Success(emptyCities));

        // Act
        var result = await _controller.GetAllCities();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetAllCities_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        _cityService.GetAllCitiesAsync(Arg.Any<CancellationToken>())
            .Returns(Result<List<CityResponse>>.Failure("Database connection failed"));

        // Act
        var result = await _controller.GetAllCities();

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetCity_WithGeorgianCityName_ReturnsCorrectCity()
    {
        // Arrange
        var cityId = 1;
        var georgianCity = new CityResponse { Id = cityId, Name = "თბილისი" };
        _cityService.GetCityByIdAsync(cityId, Arg.Any<CancellationToken>())
            .Returns(Result<CityResponse>.Success(georgianCity));

        // Act
        var result = await _controller.GetCity(cityId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.ShouldBe(200);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100)]
    public async Task GetCity_WithInvalidIds_ReturnsBadRequest(int invalidId)
    {
        // Arrange & Act
        var result = await _controller.GetCity(invalidId);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetCity_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = 9999;
        _cityService.GetCityByIdAsync(nonExistentId, Arg.Any<CancellationToken>())
            .Returns(Result<CityResponse>.Failure("City not found"));

        // Act
        var result = await _controller.GetCity(nonExistentId);

        // Assert
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAllCities_CallsServiceOnce()
    {
        // Arrange
        _cityService.GetAllCitiesAsync(Arg.Any<CancellationToken>())
            .Returns(Result<List<CityResponse>>.Success(new List<CityResponse>()));

        // Act
        await _controller.GetAllCities();

        // Assert
        await _cityService.Received(1).GetAllCitiesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCity_CallsServiceWithCorrectId()
    {
        // Arrange
        var cityId = 42;
        var city = new CityResponse { Id = cityId, Name = "Test City" };
        _cityService.GetCityByIdAsync(cityId, Arg.Any<CancellationToken>())
            .Returns(Result<CityResponse>.Success(city));

        // Act
        await _controller.GetCity(cityId);

        // Assert
        await _cityService.Received(1).GetCityByIdAsync(cityId, Arg.Any<CancellationToken>());
    }
}