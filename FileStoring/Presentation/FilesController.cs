using Application;
using Microsoft.AspNetCore.Mvc;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FilesController(IFileService fileService) =>
            _fileService = fileService;
        
        [HttpPost]
        [Route("/api/files")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var entry = await _fileService.SaveFileAsync(
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length
            );
            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }

        [HttpGet]
        public async Task<IActionResult> List() =>
            Ok(await _fileService.ListFilesAsync());

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var e = await _fileService.GetFileAsync(id);
            return e is null ? NotFound() : Ok(e);
        }

        [HttpGet("{id:guid}/content")]
        public async Task<IActionResult> Content(Guid id)
        {
            var stream = await _fileService.GetFileContentAsync(id);
            return stream is null
                ? NotFound()
                : File(stream, "text/plain");
        }
    }
}