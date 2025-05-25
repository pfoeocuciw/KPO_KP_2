using Domain;

namespace Application;

public interface IFileService
{
    Task<StoredFile> SaveFileAsync(Stream content, string fileName, string contentType, long size);

    Task<IEnumerable<StoredFile>> ListFilesAsync();
    Task<StoredFile?> GetFileAsync(Guid id);
    Task<Stream?> GetFileContentAsync(Guid id);
}