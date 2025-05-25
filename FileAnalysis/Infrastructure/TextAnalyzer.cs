using Application;
using System.Text.RegularExpressions;

namespace Infrastructure;

public class TextAnalyzer : ITextAnalyzer
{
    public (int paragraphs, int words, int characters) Analyze(string text)
    {
        var paras = text.Split(new[]{ "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
        var words = Regex.Matches(text, @"\b\w+\b").Count;
        var chars = text.Length;
        return (paras, words, chars);
    }

    public IEnumerable<string> Tokenize(string text) =>
        Regex.Matches(text.ToLowerInvariant(), @"\b\w+\b")
            .Select(m => m.Value);

    public double JaccardSimilarity(string a, string b)
    {
        var s1 = new HashSet<string>(Tokenize(a));
        var s2 = new HashSet<string>(Tokenize(b));
        var inter = s1.Intersect(s2).Count();
        var uni   = s1.Union(s2).Count();
        return uni == 0 ? 0 : (double)inter / uni;
    }
}