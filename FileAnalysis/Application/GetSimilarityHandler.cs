using MediatR;

namespace Application;

internal class GetSimilarityHandler : IRequestHandler<GetSimilarityQuery, SimilarityResultDto>
{
    private readonly IHttpClientFactory _http;
    private readonly ITextAnalyzer      _analyzer;

    public GetSimilarityHandler(IHttpClientFactory http, ITextAnalyzer analyzer)
    {
        _http     = http;
        _analyzer = analyzer;
    }

    public async Task<SimilarityResultDto> Handle(GetSimilarityQuery q, CancellationToken ct)
    {
        var client = _http.CreateClient("storage");

        var t1 = await Fetch(q.FileId1);
        var t2 = await Fetch(q.FileId2);
        var sim = _analyzer.JaccardSimilarity(t1, t2);
        return new SimilarityResultDto(q.FileId1, q.FileId2, sim);

        async Task<string> Fetch(Guid id)
        {
            var r = await client.GetAsync($"/api/files/{id}/content", ct);
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadAsStringAsync(ct);
        }
    }
}