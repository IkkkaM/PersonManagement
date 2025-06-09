namespace PersonDirectory.Api.Controllers;

public class FilesController : BaseApiController
{
    private readonly IFileService _fileService;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public FilesController(
        IFileService fileService,
        IWebHostEnvironment environment,
        IConfiguration configuration,
        IStringLocalizer<FilesController> localizer) : base(localizer)
    {
        _fileService = fileService;
        _environment = environment;
        _configuration = configuration;
    }

    /// <summary>
    /// Serve uploaded images. fileName should countaint extension of file
    /// </summary>
    [HttpGet("images/{fileName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return BadRequest(new ApiResponse { Success = false, Message = "File name is required" });

        var imagePath = Path.Combine("images", fileName);

        if (!await _fileService.ImageExistsAsync(imagePath))
            return NotFound(new ApiResponse { Success = false, Message = "Image not found" });

        var fullPath = Path.Combine(_environment.WebRootPath, "uploads", "images", fileName);

        var contentType = GetContentType(fileName);
        return PhysicalFile(fullPath, contentType);
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        var allowedExtensions = _configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>();

        if (allowedExtensions != null && !allowedExtensions.Contains(extension))
        {
            return "application/octet-stream";
        }

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
