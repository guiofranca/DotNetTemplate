using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Template.Core.Interfaces;

namespace Template.Infrastructure.FileStorage;

public class FileSystemStorage : IFileStorage
{
    private string RootPath { get; set; }
    private readonly ILogger<FileSystemStorage> _logger;

    public FileSystemStorage(IConfiguration configuration, ILogger<FileSystemStorage> logger)
    {
        var root = configuration["FileStorage:RootPath"];
        if (root == null) throw new ArgumentNullException("FileStorage:RootPath");

        RootPath = root;
        _logger = logger;
    }

    public void Save(string file, Stream stream)
    {
        Directory.CreateDirectory(Path.Combine(RootPath));
        var filePath = Path.Combine(RootPath, file);
        var fileExists = File.Exists(filePath);

        if (fileExists)
        {
            _logger.LogDebug("File {filePath} already exists!", filePath);
            return;
        }

        using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write);
        stream.CopyTo(fileStream);

        //await File.WriteAllBytesAsync(filePath, stream.cop);
        _logger.LogDebug("File {filePath} saved!", filePath);
    }

    public void Delete(string file)
    {
        var filePath = Path.Combine(RootPath, file);
        var fileDoesntExists = !File.Exists(filePath);

        if (fileDoesntExists)
        {
            _logger.LogDebug("File {filePath} doesn't exists to be deleted!", filePath);
            return;
        }

        File.Delete(filePath);
        _logger.LogDebug("File {filePath} deleted!", filePath);
    }
}
