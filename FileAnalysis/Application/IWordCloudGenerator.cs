namespace Application;

public interface IWordCloudGenerator
{
    byte[] Generate(string[] tokens);
}