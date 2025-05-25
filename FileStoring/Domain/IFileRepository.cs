namespace Domain;

public interface IFileRepository
{
    Task<StoredFile> AddAsync(Stream content, string fileName, string contentType, long size);
    Task<IEnumerable<StoredFile>> ListAsync();
    Task<StoredFile?> GetByIdAsync(Guid id);
    Task<Stream?> GetContentAsync(Guid id);
}