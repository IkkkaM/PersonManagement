namespace PersonDirectory.Application.Interfaces;

public interface IFileService
{
    Task<Result<string>> SaveImageAsync(Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task<Result> DeleteImageAsync(string imagePath, CancellationToken cancellationToken = default);
    Task<bool> ImageExistsAsync(string imagePath, CancellationToken cancellationToken = default);
    string GetImageUrl(string imagePath);
}