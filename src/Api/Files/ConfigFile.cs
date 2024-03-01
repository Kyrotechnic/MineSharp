namespace MineSharp.Api.Files;

public class ConfigFile
{
    private CFile File;
    private Dictionary<string, object> Values = new();
    public ConfigFile(CFile file)
    {
        this.File = file;

        Load();
    }

    public void Load()
    {
        string[] strings = File.ReadAllLines();

        foreach (string line in strings)
        {
            string[] split = line.Split("=");

            Values.Add(split[0], split[1]);
        }
    }

    public void Save()
    {
        string[] final = new string[Values.Count];

        int count = 0;

        foreach (KeyValuePair<string, object> valuePair in Values.ToArray())
        {
            final[count] = valuePair.Key + "=" + valuePair.Value;

            count++;
        }

        File.WriteAllLines(final);
    }

    public T Get<T>(string key, T def)
    {
        if (Values.ContainsKey(key))
            return (T) Values[key];
        return Set(key, def);
    }

    public T Set<T>(string key, T value)
    {
        Values[key] = value!;

        return value;
    }
}