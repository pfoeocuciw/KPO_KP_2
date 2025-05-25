namespace Application;

public interface ITextAnalyzer
{
    (int paragraphs, int words, int characters) Analyze(string text);
    IEnumerable<string> Tokenize(string text);
    double JaccardSimilarity(string a, string b);
}