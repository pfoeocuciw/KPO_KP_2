namespace Domain;

public readonly record struct FileId(Guid Value)
{
    public static FileId New() => new(Guid.NewGuid());
}