namespace PersonDirectory.Api.Tests.Controllers;

public class FilesControllerTests
{
    private readonly IFileService _fileService;
    private readonly IWebHostEnvironment _environment;
    private readonly IStringLocalizer<FilesController> _localizer;
    private readonly FilesController _controller;
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

    public FilesControllerTests()
    {
        _fileService = Substitute.For<IFileService>();
        _environment = Substitute.For<IWebHostEnvironment>();
        _configuration = Substitute.For<Microsoft.Extensions.Configuration.IConfiguration>();
        _localizer = Substitute.For<IStringLocalizer<FilesController>>();
        _controller = new FilesController(_fileService, _environment, _configuration, _localizer);

        // Setup environment
        _environment.WebRootPath.Returns("/app/wwwroot");
    }

    [Fact]
    public async Task GetImage_WithValidFileName_ReturnsPhysicalFile()
    {
        // Arrange
        var fileName = "test-image.jpg";
        var imagePath = Path.Combine("images", fileName);
        _fileService.ImageExistsAsync(imagePath).Returns(true);

        // Act
        var result = await _controller.GetImage(fileName);

        // Assert
        result.ShouldBeOfType<PhysicalFileResult>();
        var fileResult = result as PhysicalFileResult;
        fileResult!.ContentType.ShouldBe("image/jpeg");
    }

    [Fact]
    public async Task GetImage_WhenFileNotExists_ReturnsNotFound()
    {
        // Arrange
        var fileName = "non-existent.jpg";
        var imagePath = Path.Combine("images", fileName);
        _fileService.ImageExistsAsync(imagePath).Returns(false);

        // Act
        var result = await _controller.GetImage(fileName);

        // Assert
        result.ShouldBeOfType<NotFoundObjectResult>();
    }

    [Theory]
    [InlineData("image.jpg", "image/jpeg")]
    [InlineData("image.jpeg", "image/jpeg")]
    [InlineData("image.png", "image/png")]
    [InlineData("image.gif", "image/gif")]
    [InlineData("image.bmp", "image/bmp")]
    [InlineData("image.unknown", "application/octet-stream")]
    public async Task GetImage_WithDifferentExtensions_ReturnsCorrectContentType(string fileName, string expectedContentType)
    {
        // Arrange
        var imagePath = Path.Combine("images", fileName);
        _fileService.ImageExistsAsync(imagePath).Returns(true);

        // Act
        var result = await _controller.GetImage(fileName);

        // Assert
        result.ShouldBeOfType<PhysicalFileResult>();
        var fileResult = result as PhysicalFileResult;
        fileResult!.ContentType.ShouldBe(expectedContentType);
    }
}