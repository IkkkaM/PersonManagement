namespace PersonDirectory.Api.Tests.Controllers;

public class PersonConnectionTests
{
    private readonly IPersonService _personService;
    private readonly IFileService _fileService;
    private readonly IStringLocalizer<PersonController> _localizer;
    private readonly PersonController _controller;

    public PersonConnectionTests()
    {
        _personService = Substitute.For<IPersonService>();
        _fileService = Substitute.For<IFileService>();
        _localizer = Substitute.For<IStringLocalizer<PersonController>>();
        _controller = new PersonController(_personService, _fileService, _localizer);
    }

    [Fact]
    public async Task AddConnection_WithValidColleagueConnection_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 2,
            ConnectionType = ConnectionType.Colleague
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WithValidRelativeConnection_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 3,
            ConnectionType = ConnectionType.Relative
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WithValidAcquaintanceConnection_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 4,
            ConnectionType = ConnectionType.Acquaintance
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WithValidOtherConnection_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 5,
            ConnectionType = ConnectionType.Other
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task AddConnection_WithInvalidPersonId_ReturnsBadRequest(int invalidPersonId)
    {
        // Arrange
        var request = new PersonConnectionRequest
        {
            PersonId = invalidPersonId,
            ConnectedPersonId = 2,
            ConnectionType = ConnectionType.Colleague
        };

        // Act
        var result = await _controller.AddConnection(invalidPersonId, request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WithMismatchedPersonIds_ReturnsBadRequest()
    {
        // Arrange
        var routePersonId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = 2, // Different from route
            ConnectedPersonId = 3,
            ConnectionType = ConnectionType.Colleague
        };

        // Act
        var result = await _controller.AddConnection(routePersonId, request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WhenServiceFails_ReturnsBadRequest()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 2,
            ConnectionType = ConnectionType.Colleague
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Failure("Connection already exists"));

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RemoveConnection_WithValidIds_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var connectedPersonId = 2;
        _personService.RemovePersonConnectionAsync(personId, connectedPersonId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.RemoveConnection(personId, connectedPersonId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    [InlineData(0, 0)]
    public async Task RemoveConnection_WithInvalidIds_ReturnsBadRequest(int personId, int connectedPersonId)
    {
        // Arrange & Act
        var result = await _controller.RemoveConnection(personId, connectedPersonId);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RemoveConnection_WhenConnectionNotExists_ReturnsBadRequest()
    {
        // Arrange
        var personId = 1;
        var connectedPersonId = 2;
        _personService.RemovePersonConnectionAsync(personId, connectedPersonId, Arg.Any<CancellationToken>())
            .Returns(Result.Failure("Connection not found"));

        // Act
        var result = await _controller.RemoveConnection(personId, connectedPersonId);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task AddConnection_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 2,
            ConnectionType = ConnectionType.Colleague
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _controller.AddConnection(personId, request);

        // Assert
        await _personService.Received(1).AddPersonConnectionAsync(
            Arg.Is<PersonConnectionRequest>(r =>
                r.PersonId == personId &&
                r.ConnectedPersonId == 2 &&
                r.ConnectionType == ConnectionType.Colleague),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveConnection_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var personId = 1;
        var connectedPersonId = 2;
        _personService.RemovePersonConnectionAsync(personId, connectedPersonId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _controller.RemoveConnection(personId, connectedPersonId);

        // Assert
        await _personService.Received(1).RemovePersonConnectionAsync(
            personId,
            connectedPersonId,
            Arg.Any<CancellationToken>());
    }
}