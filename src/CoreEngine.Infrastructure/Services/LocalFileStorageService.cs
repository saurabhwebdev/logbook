using CoreEngine.Application.Common.Interfaces;

namespace CoreEngine.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService()
    {
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        var filePath = Path.Combine(_basePath, fileName);
        var directory = Path.GetDirectoryName(filePath);
        if (directory != null && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        using var outputStream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(outputStream, ct);
        return filePath;
    }

    public Task<Stream> GetFileAsync(string storagePath, CancellationToken ct = default)
    {
        if (!File.Exists(storagePath))
            throw new FileNotFoundException("File not found", storagePath);

        Stream stream = new FileStream(storagePath, FileMode.Open, FileAccess.Read);
        return Task.FromResult(stream);
    }

    public Task DeleteFileAsync(string storagePath, CancellationToken ct = default)
    {
        if (File.Exists(storagePath))
            File.Delete(storagePath);
        return Task.CompletedTask;
    }
}
