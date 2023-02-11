using Microsoft.Extensions.Logging;
using Template.Application.DTO.BlogComment;
using Template.Application.DTO.StoredFile;
using Template.Application.Result;
using Template.Application.Services.Shared;
using Template.Domain.Interfaces;
using Template.Domain.Interfaces.Repositories;
using Template.Domain.Models;

namespace Template.Application.Services;

public class StoredFileService : BaseService<StoredFileModel>
{
    private readonly IStoredFileRepository _storedFileRepository;
    private readonly IFileStorage _fileStorage;
    public StoredFileService(IUnitOfWork unitOfWork,
                             IErrorNotificator errorNotificator,
                             ICacheService cache,
                             ILogger<StoredFileService> logger,
                             IGlobalizer globalizer,
                             IStoredFileRepository storedFileRepository,
                             IFileStorage fileStorage) : base(unitOfWork, errorNotificator, cache, logger, globalizer)
    {
        _storedFileRepository = storedFileRepository;
        _fileStorage = fileStorage;
    }

    public async Task<IServiceResult<StoredFileModel>> GetAsync(Guid id)
    {
        var storedFile = await _cache.RememberModelAsync(id, _storedFileRepository.FindAsync);
        if (storedFile == null) return NotFoundResult(_g["File not found"]);

        StoredFileModel model = MapToModel(storedFile);

        return OkResult(model);
    }

    public async Task<IServiceResult<IEnumerable<StoredFileModel>>> GetAllFromOwnerAsync(string owner, Guid ownerId)
    {
        var storedFiles = await _storedFileRepository.GetAllFromOwnerAsync(owner, ownerId);
        var models = storedFiles.Select(s => MapToModel(s));

        return OkResult(models);
    }
    public async Task<IServiceResult<StoredFileModel>> GetOneFromOwnerAsync(string owner, Guid ownerId)
    {
        var storedFile = await _storedFileRepository.GetOneFromOwnerAsync(owner, ownerId);
        if (storedFile == null) return NotFoundResult(_g["File not found"]);

        var model = MapToModel(storedFile);

        return OkResult(model);
    }

    public async Task<IServiceResult<StoredFileModel>> UploadAsync(Stream stream, string fileName)
    {
        StoredFile storedFile = new()
        {
            Size = stream.Length,
            Name = fileName,
        };

        try
        {
            await _storedFileRepository.CreateAsync(storedFile);
            _fileStorage.Save(storedFile.Id.ToString(), stream);
        }
        finally
        {
            stream.Dispose();
        }

        var model = MapToModel(storedFile);

        return CreatedResult(model);
    }

    public async Task<IServiceResult<StoredFileModel>> DeleteAsync(Guid Id)
    {
        var success = await _storedFileRepository.DeleteAsync(Id);
        if (success == false) return FailureResult(_g["File not found"]);

        _fileStorage.Delete(Id.ToString());

        return DeletedResult(message: _g["File {Id} deleted", Id.ToString()]);
    }

    public async Task<IServiceResult<StoredFileModel>> AssignFileAsync(Guid Id, AssignFileRequest assignFileRequest)
    {
        var storedFile = await _storedFileRepository.FindAsync(Id);
        if(storedFile == null) return NotFoundResult(_g["File not found"]);

        storedFile.Owner = assignFileRequest.Owner;
        storedFile.OwnerId = assignFileRequest.OwnerId;

        await _storedFileRepository.UpdateAsync(storedFile);

        var model = MapToModel(storedFile);

        return UpdatedResult(model, _g["File assigned"]);
    }

    private static StoredFileModel MapToModel(StoredFile storedFile)
        => new()
        {
            Id = storedFile.Id,
            Name = storedFile.Name,
            Size = storedFile.Size,
            Owner = storedFile.Owner,
            OwnerId = storedFile.OwnerId,
            CreatedAt = storedFile.CreatedAt,
            UpdatedAt = storedFile.UpdatedAt,
        };
}
