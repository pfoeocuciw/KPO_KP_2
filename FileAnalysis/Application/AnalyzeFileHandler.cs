using System.Net.Http.Json;
using Domain;
using MediatR;

namespace Application
{
    internal class AnalyzeFileHandler : IRequestHandler<AnalyzeFileCommand, AnalysisResultDto>
    {
        private readonly HttpClient          _storageClient;
        private readonly HttpClient          _analysisStorageClient;
        private readonly HttpClient          _wcClient;
        private readonly ITextAnalyzer       _analyzer;
        private readonly IAnalysisRepository _repo;

        public AnalyzeFileHandler(
            IHttpClientFactory http,
            ITextAnalyzer analyzer,
            IAnalysisRepository repo)
        {
            _storageClient         = http.CreateClient("storage");
            _analysisStorageClient = http.CreateClient("storage");
            _wcClient              = http.CreateClient("wordcloudapi");
            _analyzer              = analyzer;
            _repo                  = repo;
        }

        public async Task<AnalysisResultDto> Handle(AnalyzeFileCommand cmd, CancellationToken ct)
        {
            var existing = await _repo.GetByFileIdAsync(new FileId(cmd.FileId), ct);
            if (existing is not null)
            {
                return new AnalysisResultDto(
                    existing.Id,
                    existing.FileId.Value,
                    existing.Paragraphs,
                    existing.Words,
                    existing.Characters,
                    existing.WordCloudLocation,
                    existing.CreatedUtc
                );
            }

            var textResp = await _storageClient.GetAsync($"/api/files/{cmd.FileId}/content", ct);
            textResp.EnsureSuccessStatusCode();
            var text = await textResp.Content.ReadAsStringAsync(ct);

            var (paragraphs, words, chars) = _analyzer.Analyze(text);

            var tokens = _analyzer.Tokenize(text).Where(t => t.Length > 1);
            var allText = System.Net.WebUtility.UrlEncode(string.Join(' ', tokens));
            var qcUrl = $"?text={allText}&format=png&width=800&height=600&fontScale=15.0";
            var wcResp = await _wcClient.GetAsync(qcUrl, ct);
            wcResp.EnsureSuccessStatusCode();
            var pngBytes = await wcResp.Content.ReadAsByteArrayAsync(ct);

            using var form = new MultipartFormDataContent();
            form.Add(new ByteArrayContent(pngBytes), "file", $"{cmd.FileId}.png");
            var uploadResp = await _analysisStorageClient.PostAsync("/api/files", form, ct);
            uploadResp.EnsureSuccessStatusCode();
            var wcFile = await uploadResp.Content.ReadFromJsonAsync<FileInfoDto>(cancellationToken: ct);

            var entry = new AnalysisEntry(
                new FileId(cmd.FileId),
                paragraphs, words, chars,
                wcFile!.Location
            );
            await _repo.AddAsync(entry, ct);

            return new AnalysisResultDto(
                entry.Id,
                entry.FileId.Value,
                entry.Paragraphs,
                entry.Words,
                entry.Characters,
                entry.WordCloudLocation,
                entry.CreatedUtc
            );
        }
    }
}