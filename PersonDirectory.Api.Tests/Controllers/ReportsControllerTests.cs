namespace PersonDirectory.Api.Tests.Controllers;

public class ReportsControllerTests
{
    private readonly IPersonService _personService;
    private readonly IStringLocalizer<ReportsController> _localizer;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _personService = Substitute.For<IPersonService>();
        _localizer = Substitute.For<IStringLocalizer<ReportsController>>();
        _controller = new ReportsController(_personService, _localizer);
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WithMultiplePersons_ReturnsOkResult()
    {
        // Arrange
        var report = new PersonConnectionReportResponse
        {
            PersonConnections = new List<PersonConnectionSummary>
                {
                    new()
                    {
                        PersonId = 1,
                        FirstName = "გიორგი",
                        LastName = "მესხი",
                        ConnectionCounts = new Dictionary<ConnectionType, int>
                        {
                            { ConnectionType.Colleague, 5 },
                            { ConnectionType.Relative, 3 }
                        },
                        TotalConnections = 8
                    },
                    new()
                    {
                        PersonId = 2,
                        FirstName = "ნინო",
                        LastName = "ლომიძე",
                        ConnectionCounts = new Dictionary<ConnectionType, int>
                        {
                            { ConnectionType.Acquaintance, 2 },
                            { ConnectionType.Other, 1 }
                        },
                        TotalConnections = 3
                    }
                }
        };
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Success(report));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WithSinglePerson_ReturnsCorrectData()
    {
        // Arrange
        var report = new PersonConnectionReportResponse
        {
            PersonConnections = new List<PersonConnectionSummary>
                {
                    new()
                    {
                        PersonId = 1,
                        FirstName = "John",
                        LastName = "Doe",
                        ConnectionCounts = new Dictionary<ConnectionType, int>
                        {
                            { ConnectionType.Colleague, 10 }
                        },
                        TotalConnections = 10
                    }
                }
        };
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Success(report));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WithAllConnectionTypes_ReturnsCompleteData()
    {
        // Arrange
        var report = new PersonConnectionReportResponse
        {
            PersonConnections = new List<PersonConnectionSummary>
                {
                    new()
                    {
                        PersonId = 1,
                        FirstName = "ანა",
                        LastName = "კაკაბაძე",
                        ConnectionCounts = new Dictionary<ConnectionType, int>
                        {
                            { ConnectionType.Colleague, 5 },
                            { ConnectionType.Relative, 3 },
                            { ConnectionType.Acquaintance, 7 },
                            { ConnectionType.Other, 2 }
                        },
                        TotalConnections = 17
                    }
                }
        };
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Success(report));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WithNoConnections_ReturnsEmptyReport()
    {
        // Arrange
        var report = new PersonConnectionReportResponse
        {
            PersonConnections = new List<PersonConnectionSummary>
                {
                    new()
                    {
                        PersonId = 1,
                        FirstName = "იოანე",
                        LastName = "ხუციშვილი",
                        ConnectionCounts = new Dictionary<ConnectionType, int>(),
                        TotalConnections = 0
                    }
                }
        };
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Success(report));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WithEmptyDatabase_ReturnsEmptyReport()
    {
        // Arrange
        var emptyReport = new PersonConnectionReportResponse
        {
            PersonConnections = new List<PersonConnectionSummary>()
        };
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Success(emptyReport));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Failure("Database connection failed"));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WhenDataCorruption_ReturnsBadRequest()
    {
        // Arrange
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Failure("Data integrity error"));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WhenTimeout_ReturnsBadRequest()
    {
        // Arrange
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.Failure("Operation timeout"));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPersonConnectionsReport_WithValidationErrors_ReturnsBadRequest()
    {
        // Arrange
        var validationErrors = new List<string> { "Invalid report parameters", "Access denied" };
        _personService.GetConnectionReportAsync(Arg.Any<CancellationToken>())
            .Returns(Result<PersonConnectionReportResponse>.ValidationFailure(validationErrors));

        // Act
        var result = await _controller.GetPersonConnectionsReport();

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }
}
