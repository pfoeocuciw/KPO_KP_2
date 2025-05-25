namespace Domain;

public class StoredFile
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long Size { get; private set; }
    public DateTime UploadedUtc { get; private set; }
    
    public string Location    { get; set; }

    public StoredFile(Guid id, string fileName, string contentType, long size, string location)
    {
        Id = id;
        FileName = fileName;
        ContentType = contentType;
        Size = size;
        UploadedUtc = DateTime.UtcNow;
        Location = location;
    }
}