using MediatR;

namespace Application;

public record AnalysisResultDto(
    Guid   AnalysisId,
    Guid   FileId,
    int    Paragraphs,
    int    Words,
    int    Characters,
    string WordCloudLocation,
    DateTime CreatedUtc
);

public record SimilarityResultDto(Guid FileId1, Guid FileId2, double Similarity);

public record AnalyzeFileCommand(Guid FileId) : IRequest<AnalysisResultDto>;

public record GenerateWordCloudQuery(Guid FileId) : IRequest<byte[]?>;

public record GetAnalysisQuery(Guid FileId) : IRequest<AnalysisResultDto?>;

public record GetSimilarityQuery(Guid FileId1, Guid FileId2) : IRequest<SimilarityResultDto>;