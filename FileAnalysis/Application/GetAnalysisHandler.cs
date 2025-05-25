using Domain;
using MediatR;

namespace Application;

internal class GetAnalysisHandler : IRequestHandler<GetAnalysisQuery, AnalysisResultDto?>
{
    private readonly IAnalysisRepository _repo;
    public GetAnalysisHandler(IAnalysisRepository repo) => _repo = repo;

    public async Task<AnalysisResultDto?> Handle(GetAnalysisQuery q, CancellationToken ct)
    {
        var entry = await _repo.GetByFileIdAsync(new FileId(q.FileId));
        return entry is null
            ? null
            : new AnalysisResultDto(entry.Id, entry.FileId.Value, entry.Paragraphs, entry.Words,
                entry.Characters, entry.WordCloudLocation, entry.CreatedUtc);
    }
}