namespace Domain;

public interface IAnalysisRepository
{
    Task AddAsync(AnalysisEntry entry, CancellationToken ct = default);
    Task<AnalysisEntry?> GetByFileIdAsync(FileId fileId, CancellationToken ct = default);
}