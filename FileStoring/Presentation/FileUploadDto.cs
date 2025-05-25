using System.ComponentModel.DataAnnotations;

namespace Presentation
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}