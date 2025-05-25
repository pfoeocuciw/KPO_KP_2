using Domain;

namespace Application
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _repo;

        public FileService(IFileRepository repo) => _repo = repo;

        public Task<StoredFile> SaveFileAsync(Stream content, string fileName, string contentType, long size)
        {
            return _repo.AddAsync(content, fileName, contentType, size);
        }

        public Task<IEnumerable<StoredFile>> ListFilesAsync() =>
            _repo.ListAsync();

        public Task<StoredFile?> GetFileAsync(Guid id) =>
            _repo.GetByIdAsync(id);

        public Task<Stream?> GetFileContentAsync(Guid id) =>
            _repo.GetContentAsync(id);
    }
}