namespace MineSharp.Api.Files;

public class CFile
{
    public string path;
    public CFile(string filePath)
    {
        this.path = filePath;
    }

    public string[] ReadAllLines() => File.ReadAllLines(path);
    public byte[] ReadAllBytes() => File.ReadAllBytes(path);
    public void WriteAllLines(string[] arr) => File.WriteAllLines(path, arr);
    public void WriteAllBytes(byte[] b) => File.WriteAllBytes(path, b);
}