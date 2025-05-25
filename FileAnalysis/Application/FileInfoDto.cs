namespace Application;

public sealed record FileInfoDto(
    Guid   Id,
    string FileName,
    string ContentType,
    long   Size,
    string Location
);