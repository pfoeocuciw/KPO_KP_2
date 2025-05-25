using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation;
[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private readonly IMediator _m;
    public AnalysisController(IMediator m) => _m = m;

    [HttpPost("{fileId:guid}")]
    public async Task<IActionResult> Analyze(Guid fileId)
    {
        var dto = await _m.Send(new AnalyzeFileCommand(fileId));
        return dto is null
            ? NotFound()
            : CreatedAtAction(nameof(Get), new { fileId }, dto);
    }

    [HttpGet("{fileId:guid}")]
    public async Task<AnalysisResultDto?> Get(Guid fileId) =>
        await _m.Send(new GetAnalysisQuery(fileId));

    [HttpGet("similarity/{a:guid}/{b:guid}")]
    public async Task<SimilarityResultDto> Similarity(Guid a, Guid b) =>
        await _m.Send(new GetSimilarityQuery(a, b));

    [HttpGet("wordcloud/{fileId:guid}")]
    public async Task<IActionResult> WordCloud(Guid fileId)
    {
        var png = await _m.Send(new GenerateWordCloudQuery(fileId));
        return png is null
            ? NotFound()
            : File(png, "image/png");
    }
}