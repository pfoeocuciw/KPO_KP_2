namespace Infrastructure;

public class FileEntity
{
    public Guid   Id          { get; set; }
    public string Hash        { get; set; } = null!;
    public string FileName    { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long   Size        { get; set; }
    public string Location    { get; set; } = null!;
    public DateTime UploadedUtc { get; set; }
}