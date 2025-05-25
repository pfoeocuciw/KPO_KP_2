using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class SqlAnalysisRepository : IAnalysisRepository
    {
        private readonly FileAnalysisDbContext _ctx;
        public SqlAnalysisRepository(FileAnalysisDbContext ctx) => _ctx = ctx;

        public async Task AddAsync(AnalysisEntry entry, CancellationToken ct = default)
        {
            _ctx.Analyses.Add(new AnalysisEntity {
                Id          = entry.Id,
                FileId      = entry.FileId.Value,
                Paragraphs  = entry.Paragraphs,
                Words       = entry.Words,
                Characters  = entry.Characters,
                WordCloudLocation  = entry.WordCloudLocation,
                CreatedUtc  = entry.CreatedUtc
            });
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task<AnalysisEntry?> GetByFileIdAsync(FileId fileId, CancellationToken ct = default)
        {
            var e = await _ctx.Analyses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FileId == fileId.Value, ct);
            if (e is null) return null;

            return new AnalysisEntry(
                new FileId(e.FileId),
                e.Paragraphs,
                e.Words,
                e.Characters,
                e.WordCloudLocation
            )
            {
                Id = e.Id,
                CreatedUtc = e.CreatedUtc
            };
        }
    }
}