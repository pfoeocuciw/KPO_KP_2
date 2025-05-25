using MediatR;
using System.Net;

namespace Application;
internal class GenerateWordCloudHandler : IRequestHandler<GenerateWordCloudQuery, byte[]?>
{
    private readonly IHttpClientFactory _http;
    private readonly ITextAnalyzer      _analyzer;

    public GenerateWordCloudHandler(IHttpClientFactory http,
        ITextAnalyzer analyzer)
    {
        _http     = http;
        _analyzer = analyzer;
    }

    public async Task<byte[]?> Handle(GenerateWordCloudQuery q, CancellationToken ct)
    {
        var storage = _http.CreateClient("storage");
        var r1 = await storage.GetAsync($"/api/files/{q.FileId}/content", ct);
        if (!r1.IsSuccessStatusCode) return null;
        var text = await r1.Content.ReadAsStringAsync(ct);

        var tokens = _analyzer.Tokenize(text)
            .Where(w => w.Length > 1)
            .ToArray();
        if (tokens.Length == 0) return null;

        var allText = WebUtility.UrlEncode(string.Join(' ', tokens));
        var url     = $"?text={allText}&format=png&width=800&height=600&fontScale=15.0";

        var qc = _http.CreateClient("wordcloudapi");
        var r2 = await qc.GetAsync(url, ct);
        if (!r2.IsSuccessStatusCode) return null;

        return await r2.Content.ReadAsByteArrayAsync(ct);
    }
}