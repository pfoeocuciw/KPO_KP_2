using System.Security.Cryptography;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure
{
    public class FileRepository : IFileRepository
    {
        private readonly FileStorageDbContext _ctx;
        private readonly string _root;
        private readonly ILogger<FileRepository> _log;

        public FileRepository(FileStorageDbContext ctx,
                              IConfiguration cfg,
                              ILogger<FileRepository> log)
        {
            _ctx  = ctx;
            _root = cfg["FileStoragePath"] ?? "Storage";
            Directory.CreateDirectory(_root);
            _log  = log;
        }

        public async Task<StoredFile> AddAsync(Stream content,
                                              string name,
                                              string contentType,
                                              long size)
        {
            _log.LogInformation("≫ [Repository] AddAsync: {Name}, size {Size}", name, size);

            byte[] data;
            using (var ms = new MemoryStream())
            {
                await content.CopyToAsync(ms);
                data = ms.ToArray();
            }
            var hash = ComputeSha256(data);

            var existing = await _ctx.Files
                .AsNoTracking()
                .SingleOrDefaultAsync(f => f.Hash == hash);

            if (existing is not null)
            {
                _log.LogInformation("≫ [Repository] Duplicate detected, returning existing Id={Id}", existing.Id);
                return MapToDomain(existing);
            }

            var id      = Guid.NewGuid();
            var ext     = Path.GetExtension(name);
            var fileName = $"{id}{ext}";
            var path    = Path.Combine(_root, fileName);
            await File.WriteAllBytesAsync(path, data);

            var location = $"/storage/content/{fileName}";
            var entity = new FileEntity
            {
                Id           = id,
                Hash         = hash,
                FileName     = name,
                ContentType  = contentType,
                Size         = size,
                Location     = location,
                UploadedUtc  = DateTime.UtcNow
            };

            _ctx.Files.Add(entity);
            await _ctx.SaveChangesAsync();
            _log.LogInformation("≫ [Repository] Saved new file Id={Id}", id);

            return MapToDomain(entity);
        }

        public async Task<IEnumerable<StoredFile>> ListAsync()
        {
            _log.LogInformation("≫ [Repository] ListAsync called");
            var entities = await _ctx.Files
                                     .AsNoTracking()
                                     .OrderBy(x => x.UploadedUtc)
                                     .ToListAsync();
            return entities.Select(MapToDomain);
        }

        public async Task<StoredFile?> GetByIdAsync(Guid id)
        {
            _log.LogInformation("≫ [Repository] GetByIdAsync: {Id}", id);
            var e = await _ctx.Files
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Id == id);
            return e is null ? null : MapToDomain(e);
        }

        public Task<Stream?> GetContentAsync(Guid id)
        {
            _log.LogInformation("≫ [Repository] GetContentAsync: {Id}", id);
            var entity = _ctx.Files
                             .AsNoTracking()
                             .FirstOrDefault(x => x.Id == id);
            if (entity is null)
                return Task.FromResult<Stream?>(null);

            var fileName = Path.GetFileName(entity.Location);
            var fullPath = Path.Combine(_root, fileName);
            if (!File.Exists(fullPath))
                return Task.FromResult<Stream?>(null);

            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<Stream?>(stream);
        }

        private static string ComputeSha256(byte[] bytes)
        {
            using var alg = SHA256.Create();
            return Convert.ToHexString(alg.ComputeHash(bytes));
        }

        private static StoredFile MapToDomain(FileEntity e) =>
            new StoredFile(e.Id, e.FileName, e.ContentType, e.Size, e.Location);
    }
}