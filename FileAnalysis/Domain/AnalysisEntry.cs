namespace Domain
{
    public class AnalysisEntry
    {
        public Guid Id { get; set; }
        public FileId FileId { get; set; }
        public int Paragraphs { get; set; }
        public int Words { get; set; }
        public int Characters { get; set; }
        public string WordCloudLocation { get; set; }
        public DateTime CreatedUtc { get; set; }

        public AnalysisEntry(FileId fileId,
            int paragraphs,
            int words,
            int characters,
            string wordCloudLocation)
        {
            Id                 = Guid.NewGuid();
            FileId             = fileId;
            Paragraphs         = paragraphs;
            Words              = words;
            Characters         = characters;
            WordCloudLocation  = wordCloudLocation ?? throw new ArgumentNullException(nameof(wordCloudLocation));
            CreatedUtc         = DateTime.UtcNow;
        }
    }
}