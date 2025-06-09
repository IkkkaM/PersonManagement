namespace PersonDirectory.Api.Tests.Controllers;

public class PersonControllerTests
{
    private readonly IPersonService _personService;
    private readonly IFileService _fileService;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    private readonly IStringLocalizer<PersonController> _localizer;
    private readonly PersonController _controller;

    public PersonControllerTests()
    {
        _personService = Substitute.For<IPersonService>();
        _fileService = Substitute.For<IFileService>();
        _configuration = Substitute.For<Microsoft.Extensions.Configuration.IConfiguration>();
        _localizer = Substitute.For<IStringLocalizer<PersonController>>();
        _controller = new PersonController(_personService, _fileService, _configuration, _localizer);
    }

    [Fact]
    public async Task GetPerson_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var expectedPerson = new PersonResponse
        {
            Id = personId,
            FirstName = "John",
            LastName = "Doe"
        };
        _personService.GetPersonByIdAsync(personId, Arg.Any<CancellationToken>())
            .Returns(Result<PersonResponse>.Success(expectedPerson));

        // Act
        var result = await _controller.GetPerson(personId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task GetPerson_WithInvalidId_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = 0;

        // Act
        var result = await _controller.GetPerson(invalidId);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequest = result as BadRequestObjectResult;
        badRequest!.StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task GetPerson_WhenPersonNotFound_ReturnsNotFound()
    {
        // Arrange
        var personId = 999;
        _personService.GetPersonByIdAsync(personId, Arg.Any<CancellationToken>())
            .Returns(Result<PersonResponse>.Failure("Person not found"));

        // Act
        var result = await _controller.GetPerson(personId);

        // Assert
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task CreatePerson_WithValidRequest_ReturnsCreatedResult()
    {
        // Arrange
        var request = new PersonCreateRequest
        {
            FirstName = "John",
            LastName = "Doe",
            PersonalNumber = "12345678901",
            CityId = 1
        };
        var expectedPerson = new PersonResponse { Id = 1, FirstName = "John", LastName = "Doe" };
        _personService.CreatePersonAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result<PersonResponse>.Success(expectedPerson));

        // Act
        var result = await _controller.CreatePerson(request);

        // Assert
        result.ShouldBeOfType<CreatedAtActionResult>();
        var createdResult = result as CreatedAtActionResult;
        createdResult!.StatusCode.ShouldBe(201);
        createdResult.ActionName.ShouldBe(nameof(_controller.GetPerson));
    }

    [Fact]
    public async Task CreatePerson_WithServiceFailure_ReturnsBadRequest()
    {
        // Arrange
        var request = new PersonCreateRequest();
        _personService.CreatePersonAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result<PersonResponse>.Failure("Creation failed"));

        // Act
        var result = await _controller.CreatePerson(request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdatePerson_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var request = new PersonUpdateRequest
        {
            FirstName = "Updated",
            LastName = "Name"
        };
        var updatedPerson = new PersonResponse { Id = personId, FirstName = "Updated", LastName = "Name" };
        _personService.UpdatePersonAsync(personId, request, Arg.Any<CancellationToken>())
            .Returns(Result<PersonResponse>.Success(updatedPerson));

        // Act
        var result = await _controller.UpdatePerson(personId, request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DeletePerson_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        _personService.DeletePersonAsync(personId, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.DeletePerson(personId);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UploadPersonImage_WithValidFile_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var file = CreateMockImageFile();
        var expectedResult = new PersonResponse { Id = personId, ImagePath = "path/to/image.jpg" };

        _fileService.SaveImageAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<string>.Success("images/test.jpg"));
        _personService.UploadPersonImageAsync(personId, Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<PersonResponse>.Success(expectedResult));

        // Act
        var result = await _controller.UploadPersonImage(personId, file);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UploadPersonImage_WithInvalidFileType_ReturnsBadRequest()
    {
        // Arrange
        var personId = 1;
        var file = CreateMockFile("document.pdf", "application/pdf");

        // Act
        var result = await _controller.UploadPersonImage(personId, file);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task QuickSearch_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new PersonQuickSearchRequest
        {
            SearchTerm = "John",
            PageNumber = 1,
            PageSize = 10
        };
        var searchResult = new PagedResponse<PersonListResponse>();
        _personService.QuickSearchAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<PersonListResponse>>.Success(searchResult));

        // Act
        var result = await _controller.QuickSearch(request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = personId,
            ConnectedPersonId = 2,
            ConnectionType = Domain.Enums.ConnectionType.Colleague
        };
        _personService.AddPersonConnectionAsync(request, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task AddConnection_WithMismatchedPersonId_ReturnsBadRequest()
    {
        // Arrange
        var personId = 1;
        var request = new PersonConnectionRequest
        {
            PersonId = 2,
            ConnectedPersonId = 3
        };

        // Act
        var result = await _controller.AddConnection(personId, request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
    }

    private static IFormFile CreateMockImageFile()
    {
        return CreateMockFile("test.jpg", "image/jpeg");
    }

    private static IFormFile CreateMockFile(string fileName, string contentType)
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.ContentType.Returns(contentType);
        file.Length.Returns(1024);
        file.OpenReadStream().Returns(new MemoryStream(new byte[1024]));
        return file;
    }
}