namespace PersonDirectory.Application.Services;

public class FileService : IFileService
{
    private readonly string _uploadPath;
    private readonly string _baseUrl;
    private readonly IStringLocalizer<FileService> _localizer;

    public FileService(IConfiguration configuration, IStringLocalizer<FileService> localizer)
    {
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        _uploadPath = configuration["FileStorage:UploadPath"] ?? "uploads/images";
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/api/files";

        // Ensure upload directory exists
        Directory.CreateDirectory(_uploadPath);
    }

    public async Task<Result<string>> SaveImageAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(fileExtension))
                return Result<string>.Failure(_localizer[ErrorMessages.InvalidFileFormat]);

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadPath, uniqueFileName);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageStream.CopyToAsync(fileStream, cancellationToken);

            var relativePath = Path.Combine("images", uniqueFileName).Replace("\\", "/");
            return Result<string>.Success(relativePath);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"{_localizer[ErrorMessages.FileUploadFailed]}: {ex.Message}");
        }
    }

    public async Task<Result> DeleteImageAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return Result.Success();

            var fullPath = Path.Combine(_uploadPath, Path.GetFileName(imagePath));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"{_localizer[ErrorMessages.FileNotFound]}: {ex.Message}");
        }
    }

    public async Task<bool> ImageExistsAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fullPath = Path.Combine(_uploadPath, Path.GetFileName(imagePath));
            return File.Exists(fullPath);
        }
        catch
        {
            return false;
        }
    }

    public string GetImageUrl(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return string.Empty;

        return $"{_baseUrl}/{imagePath}";
    }
}