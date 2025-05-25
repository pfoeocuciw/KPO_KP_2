namespace Infrastructure;

public class AnalysisEntity
{
    public Guid      Id         { get; set; }
    public Guid      FileId     { get; set; }
    public int       Paragraphs { get; set; }
    public int       Words      { get; set; }
    public int       Characters { get; set; }
    public DateTime  CreatedUtc { get; set; }
    public string   WordCloudLocation  { get; init; }
}